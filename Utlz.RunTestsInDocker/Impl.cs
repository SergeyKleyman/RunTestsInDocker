using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Utlz.Helpers;
using Utlz.Logging;
using static Utlz.Logging.AdditionalContextBuilder;

namespace Utlz.RunTestsInDocker
{
	internal class Impl
	{
		private static readonly Logger Logger = LoggerSingleton.Factory.GetLogger<Impl>();

		private readonly DirectoryInfo _currentTestIterationDirectory;

		private readonly IDictionary<string, string> _envVarsForDocker;

		private readonly CommandLineOptions _options;

		private readonly DirectoryInfo _shadowCopyDirectory;

		private readonly DirectoryInfo _testResultsDirectory;

		private readonly DateTime _timeStarted;

		private IExternalExeOutputSink _currentExternalExeOutputSink = new ExternalExeOutputConsoleSink();

		private int _currentIterationNumber;

		private string _currentStepName;
		private int _numberOfFailedIterations;

		private List<string> _testProjectsToRun;

		internal Impl(CommandLineOptions options)
		{
			_timeStarted = DateTime.Now;

			_options = options;

			_shadowCopyDirectory = new DirectoryInfo(Path.Combine(_options.SharedWithDockerDirectory.FullName,
				Consts.ShadowCopyDirectoryName));

			_testResultsDirectory = new DirectoryInfo(Path.Combine(_options.SharedWithDockerDirectory.FullName,
				Consts.TestResults.DirectoryName));

			_currentTestIterationDirectory = new DirectoryInfo(Path.Combine(_testResultsDirectory.FullName,
				Consts.TestResults.CurrentIterationDirectoryName));

			if (_options.EnvVarsFile != null)
			{
				using (var streamReader = new StreamReader(_options.EnvVarsFile.FullName))
					_envVarsForDocker = PropertiesLoader.Load(streamReader);
			}
		}

		private DirectoryInfo BuildPathForIterationArchive(bool isSuccessful) =>
			new DirectoryInfo(Path.Combine(_testResultsDirectory.FullName,
				(isSuccessful
					? Consts.TestResults.SuccessfulIterationsArchiveDirectoryName
					: Consts.TestResults.FailedIterationsArchiveDirectoryName) + "\\" +
				_timeStarted.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture) + "\\" +
				_currentIterationNumber));

		internal int Run()
		{
			Logger.Debug?.Log("Entered", AdditionalContext((nameof(_options), _options)));

			var stepName = $"Tests in Docker container `{_options.DockerContainer}'";
			try
			{
				RunStep(stepName, () =>
				{
					Prepare();
					RunTestIterations();
				});
				Console.Title = $"SUCCESSFULLY completed [{stepName}]";
				return 0;
			}
			catch (Exception ex)
			{
				Console.Title = $"FAILED to complete [{stepName}] - {ex.Message}";
				Logger.Fatal?.Log(ex, $"Exception thrown from the root step [{stepName:StepName}]");
				return 1;
			}
		}

		private void Prepare()
		{
			using (new CurrentExternalExeOutputSinkSetter(this, new ExternalExeOutputFilesSink(
				Path.Combine(_options.SharedWithDockerDirectory.FullName, Consts.PrepareLogFileName),
				Path.Combine(_options.SharedWithDockerDirectory.FullName, Consts.PrepareErrOnlyLogFileName))))
			{
				RunStep("Prepare to run tests", () =>
				{
					MakeSureDirectoryExists("shadow copy", _shadowCopyDirectory);

					RunRobocopy(BuildPushStepName("Take solution snapshot"), _options.SolutionDirectory,
						_shadowCopyDirectory,
						$"/MIR /E /XD \"{_options.SharedWithDockerDirectory}\" .git .github .idea .vs bin obj");

					BuildSolution();
				});
			}
		}

		private void BuildSolution() =>
			RunStep(BuildPushStepName("Build solution"), () =>
			{
				RunInsideDockerContainer(BuildPushStepName("Pull NuGet packages"), "nuget restore ElasticApmAgent.sln");
				RunInsideDockerContainer(BuildPushStepName("MSBuild"), "msbuild");
			});

		private void RunTestIterations() =>
			RunStep(BuildPushStepName("Run test iterations"), () =>
			{
				MakeSureDirectoryExists("current iteration log", _currentTestIterationDirectory);
				DecideWhichTestProjectsToRun();
				while (true)
				{
					++_currentIterationNumber;
					RunOneTestIteration();
				}
			});

		private void DecideWhichTestProjectsToRun()
		{
			if (_options.LimitToTestProject != null)
			{
				_testProjectsToRun = new List<string> {_options.LimitToTestProject};
				return;
			}

			_testProjectsToRun = FindAllTestProjects();
		}

		private List<string> FindAllTestProjects()
		{
			var result = new List<string>();
			RunStep(BuildPushStepName("Find all test projects"), () =>
			{
				var testProjectSubdirs =
					new DirectoryInfo(Path.Combine(_shadowCopyDirectory.FullName, "test")).GetDirectories("*Tests",
						SearchOption.TopDirectoryOnly);
				foreach (var testProjectSubdir in testProjectSubdirs)
				{
					if (testProjectSubdir.Name == "Elastic.Apm.Tests")
						// Make sure if "Elastic.Apm.Tests" then it is the first
						result.Insert(0, testProjectSubdir.Name);
					else
						result.Add(testProjectSubdir.Name);
				}
			});
			return result;
		}

		private void RunOneTestIteration() => RunStep(BuildStepNameTestStats("Run one test iteration"), () =>
		{
			var isSuccessful = RunOneIterationTests();
			if (!isSuccessful) ++_numberOfFailedIterations;

			ArchiveCurrentIterationResults(isSuccessful);
		});

		private bool RunOneIterationTests()
		{
			var result = true;

			var stepName = new StringBuilder("Run one iteration tests");
			if (_options.LimitToTestProject == null)
				stepName.Append(" - ALL projects");
			else
				stepName.Append($" - only `{_options.LimitToTestProject}' project");
			if (_options.LimitToTestFilter != null) stepName.Append($" with filter `{_options.LimitToTestFilter}'");
			RunStep(BuildStepNameTestStats(stepName.ToString()), () =>
			{
				using (new CurrentExternalExeOutputSinkSetter(this, new ExternalExeOutputFilesSink(
					Path.Combine(_currentTestIterationDirectory.FullName, Consts.TestResults.LogFileName),
					Path.Combine(_currentTestIterationDirectory.FullName, Consts.TestResults.ErrorOnlyLogFileName))))
				{
					foreach (var testProject in _testProjectsToRun)
					{
						var isSuccessful = RunTestProject(testProject);
						if (isSuccessful) continue;
						Logger.Error?.Log($"Test project {testProject:TestProject} failed" +
						                  " - continuing to run the rest of the test projects" +
						                  $" but this iteration (#{_currentIterationNumber}) will be considered as FAILED");
						result = false;
					}
				}
			});
			return result;
		}

		private bool RunTestProject(string testProject)
		{
			var result = true;

			var dotTestCmdLine = new StringBuilder("dotnet test");

			dotTestCmdLine.Append($@" test\{testProject}");

			if (_options.LimitToTestFilter != null) dotTestCmdLine.Append($" --filter {_options.LimitToTestFilter}");

			dotTestCmdLine.Append($" --verbosity {_options.DotnetTestVerbosity}");
			dotTestCmdLine.Append(" --no-build");

			try
			{
				RunInsideDockerContainer(BuildStepNameTestStats(
						$"Run test project {testProject}" +
						(_options.LimitToTestFilter == null ? "" : " with filter `{_options.LimitToTestFilter}'")),
					dotTestCmdLine.ToString(), _envVarsForDocker);
			}
			catch (InvalidOperationException ex)
			{
				Logger.Error?.Log(ex, $"Some/all tests in project {testProject:TestProject} failed");
				result = false;
			}

			return result;
		}

		private void ArchiveCurrentIterationResults(bool isSuccessful) =>
			RunStep(BuildStepNameTestStats("Archive current iteration results"), () =>
			{
				var archiveDirectory = BuildPathForIterationArchive(isSuccessful);
				MakeSureDirectoryExists("current iteration results archive", archiveDirectory);
				RunRobocopy(BuildStepNameTestStats("Copy current iteration results"),
					_currentTestIterationDirectory, archiveDirectory, "");
			});

		private string BuildPushStepName(string stepName) => $"{stepName} - {_currentStepName}";

		private string BuildStepNameTestStats(string stepName)
		{
			var statsPrefix = new StringBuilder();
			if (_numberOfFailedIterations != 0) statsPrefix.Append($"FAILED {_numberOfFailedIterations}");
			if (statsPrefix.Length != 0) statsPrefix.Append(" ");
			statsPrefix.Append($"Iteration #{_currentIterationNumber}");
			return $"[{statsPrefix}] {stepName} - Tests in Docker container `{_options.DockerContainer}'";
		}

		private void RunStep(string stepName, Action action)
		{
			using (new ScopedConsoleTitle(stepName))
			{
				var parentStepName = _currentStepName;
				try
				{
					_currentExternalExeOutputSink.StartingStep(stepName);
					_currentStepName = stepName;
					Logger.Trace?.Log($"Starting step [{stepName:StepName}]...");
					action();
					Logger.Trace?.Log($"Finished step [{stepName:StepName}]");
				}
				catch (Exception ex)
				{
					Logger.Trace?.Log(ex, $"Exception thrown during step [{stepName:StepName}]");
					throw;
				}
				finally
				{
					_currentStepName = parentStepName;
					_currentExternalExeOutputSink.FinishedStep(stepName);
				}
			}
		}

		private void MakeSureDirectoryExists(string dirDesc, DirectoryInfo dirPath) =>
			RunStep(BuildPushStepName($"Make sure {dirDesc} directory (`{dirPath}') exists"), () =>
			{
				if (!dirPath.Exists) dirPath.Create();
			});

		private void RunRobocopy(string stepName, DirectoryInfo srcDir, DirectoryInfo dstDir, string options) =>
			RunExternalExe(stepName, "robocopy.exe", $"{srcDir.FullName} {dstDir.FullName} {options}",
				exitCode => exitCode <= 7);

		private void RunExternalExe(string stepName, string exePath, string exeArgs) =>
			RunExternalExe(stepName, exePath, exeArgs, exitCode => exitCode == 0);

		private void RunExternalExe(string stepName, string exePath, string exeArgs,
			Func<int, bool> isSuccessfulExitCode) =>
			RunStep(stepName, () => RunExternalExeImpl(exePath, exeArgs, isSuccessfulExitCode));

		private void RunExternalExeImpl(string exePath, string exeArgs, Func<int, bool> isSuccessfulExitCode)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = exePath,
				Arguments = exeArgs,
				UseShellExecute = false, // Set UseShellExecute to false if RedirectStandardError is true
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			using (var process = Process.Start(processStartInfo))
			{
				process.OutputDataReceived += (_, eventArgs) =>
				{
					if (eventArgs.Data != null) _currentExternalExeOutputSink.ReceiveStdoutLine(eventArgs.Data);
				};

				process.ErrorDataReceived += (_, eventArgs) =>
				{
					if (eventArgs.Data != null) _currentExternalExeOutputSink.ReceiveStderrLine(eventArgs.Data);
				};

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();

				if (!isSuccessfulExitCode(process.ExitCode))
				{
					throw new InvalidOperationException(
						$"{_currentStepName}" +
						"- External exe completed with unsuccessful exit code." +
						$"Exit code: {process.ExitCode}; " +
						$"External exe path: `{exePath}', arguments: `{exeArgs}'; ");
				}
			}
		}

		private void RunInsideDockerContainer(string stepName, string remoteCmdLine,
			IDictionary<string, string> remoteEnvVars = null)
		{
			var cmdLineArgs = new StringBuilder("exec");
			cmdLineArgs.Append($" --workdir C:/shared_with_Docker/{Consts.ShadowCopyDirectoryName}");

			if (remoteEnvVars != null)
			{
				foreach (var (name, value) in remoteEnvVars)
					cmdLineArgs.Append($" --env {name}={value}");
			}

			cmdLineArgs.Append($" {_options.DockerContainer}");

			cmdLineArgs.Append($" {remoteCmdLine}");

			RunExternalExe(stepName, "docker.exe", cmdLineArgs.ToString());
		}

		private interface IExternalExeOutputSink : IDisposable
		{
			void ReceiveStdoutLine(string line);
			void ReceiveStderrLine(string line);
			void StartingStep(string stepName);
			void FinishedStep(string stepName);
		}

		private class CurrentExternalExeOutputSinkSetter : IDisposable
		{
			private readonly IExternalExeOutputSink _oldSink;
			private readonly Impl _parent;

			internal CurrentExternalExeOutputSinkSetter(Impl parent, IExternalExeOutputSink newSink)
			{
				_parent = parent;
				_oldSink = _parent._currentExternalExeOutputSink;
				_parent._currentExternalExeOutputSink = newSink;
			}

			public void Dispose()
			{
				_parent._currentExternalExeOutputSink.Dispose();
				_parent._currentExternalExeOutputSink = _oldSink;
			}
		}

		private class ExternalExeOutputFilesSink : IExternalExeOutputSink
		{
			private readonly string _fileForErrOnly;
			private readonly StreamWriter _writerForOutAndErr;
			private DateTime _currentStepStartTime;
			private StreamWriter _writerForErrOnly;

			internal ExternalExeOutputFilesSink(string fileForOutAndErr, string fileForErrOnly)
			{
				_writerForOutAndErr = new StreamWriter(fileForOutAndErr);
				_fileForErrOnly = fileForErrOnly;
				var fileInfoForErrOnly = new FileInfo(_fileForErrOnly);
				if (fileInfoForErrOnly.Exists && fileInfoForErrOnly.Length > 0 || !fileInfoForErrOnly.Exists)
					_writerForErrOnly = new StreamWriter(fileForErrOnly);
			}

			public void StartingStep(string stepName)
			{
				_currentStepStartTime = DateTime.Now;
				WriteLineAndFlush(_writerForOutAndErr,
					"===============================================================================");
				WriteLineAndFlush(_writerForOutAndErr, "=======================================");
				WriteLineAndFlush(_writerForOutAndErr, "===================");
				WriteLineAndFlush(_writerForOutAndErr, "===");
				WriteLineAndFlush(_writerForOutAndErr,
					"=== " + $"{FormatTimestamp(_currentStepStartTime)}: Starting step [{stepName}]...");
				WriteLineAndFlush(_writerForOutAndErr, "===");
				WriteLineAndFlush(_writerForOutAndErr, "");
			}

			public void FinishedStep(string stepName)
			{
				var currentStepFinishTime = DateTime.Now;
				WriteLineAndFlush(_writerForOutAndErr, "");
				WriteLineAndFlush(_writerForOutAndErr, "===");
				WriteLineAndFlush(_writerForOutAndErr,
					"=== " + $"{FormatTimestamp(currentStepFinishTime)}: Finished step [{stepName}]." +
					$" Elapsed time: {FormatDuration(currentStepFinishTime - _currentStepStartTime)}.");
				WriteLineAndFlush(_writerForOutAndErr, "===");
				WriteLineAndFlush(_writerForOutAndErr, "===================");
				WriteLineAndFlush(_writerForOutAndErr, "=======================================");
				WriteLineAndFlush(_writerForOutAndErr,
					"===============================================================================");
			}

			public void ReceiveStdoutLine(string line) =>
				WriteLineAndFlush(_writerForOutAndErr, TextUtils.Indent(line));

			public void ReceiveStderrLine(string line)
			{
				WriteLineAndFlush(_writerForOutAndErr, TextUtils.Indent(line));
				if (_writerForErrOnly == null) _writerForErrOnly = new StreamWriter(_fileForErrOnly);
				WriteLineAndFlush(_writerForErrOnly, line);
			}

			public void Dispose()
			{
				_writerForOutAndErr?.Dispose();
				_writerForErrOnly?.Dispose();
			}

			private static void WriteLineAndFlush(StreamWriter streamWriter, string line)
			{
				streamWriter.WriteLine(line);
				streamWriter.Flush();
			}

			private static string FormatTimestamp(DateTime now) =>
				now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

			private string FormatDuration(TimeSpan duration)
			{
				var result = new StringBuilder();
				AppendIfNotZero(result, duration.Days, "day");
				AppendIfNotZero(result, duration.Hours, "hour");
				AppendIfNotZero(result, duration.Minutes, "minute");
				AppendIfNotZero(result, duration.Seconds, "second");
				if (duration.TotalSeconds < 1)
				{
					if (duration.Milliseconds == 0)
						result.Append("0 milliseconds");
					else
						AppendIfNotZero(result, duration.Milliseconds, "millisecond");
				}

				return result.ToString();

				void AppendIfNotZero(StringBuilder strBuilder, int value, string unit)
				{
					if (value != 0)
					{
						if (strBuilder.Length != 0) strBuilder.Append(" ");
						strBuilder.Append(Math.Abs(value) == 1 ? $"{value} {unit}" : $"{value} {unit}s");
					}
				}
			}
		}

		private class ExternalExeOutputConsoleSink : IExternalExeOutputSink
		{
			private const string StdOutPrefix = "           ";
			private const string StdErrPrefix = "   STDERR: ";

			public void StartingStep(string stepName)
			{
			}

			public void FinishedStep(string stepName)
			{
			}

			public void ReceiveStdoutLine(string line) =>
				Console.WriteLine(TextUtils.PrefixEveryLine(StdOutPrefix, line));

			public void ReceiveStderrLine(string line) =>
				Console.Error.WriteLine(TextUtils.PrefixEveryLine(StdErrPrefix, line));

			public void Dispose()
			{
			}
		}
	}
}

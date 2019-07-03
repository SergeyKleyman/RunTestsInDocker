using System.IO;
using CommandLine;
using Utlz.Helpers;
using Utlz.Logging;

namespace Utlz.RunTestsInDocker
{
	public class CommandLineOptions
	{
		[Option("docker_container", Required = true,
			HelpText = "Name of the Docker container in where tests should run.")]
		public string DockerContainer { get; set; }

		[Option("solution_directory", Required = true,
			HelpText = "Full path to directory containing the solution.")]
		public DirectoryInfo SolutionDirectory { get; set; }

		[Option("shared_with_docker_directory", Required = true,
			HelpText = "Full path to directory shared with Docker container.")]
		public DirectoryInfo SharedWithDockerDirectory { get; set; }

		[Option("limit_to_test_project", Required = false,
			HelpText = "Run only the tests from this test project.")]
		public string LimitToTestProject { get; set; }

		[Option("limit_to_test_filter", Required = false,
			HelpText = "Run only the tests passing this filter (passed as `--filter' to `dotnet test').")]
		public string LimitToTestFilter { get; set; }

		[Option("dotnet_test_verbosity", Required = false,
			HelpText = "Value passed as argument of `--verbosity' to `dotnet test')")]
		public string DotnetTestVerbosity { get; set; }

		[Option("env_vars_file", Required = false,
			HelpText = "File with environment variables to set for commands inside Docker container")]
		public FileInfo EnvVarsFile { get; set; }

		[Option("log_level", Required = false, HelpText = "Log level.")]
		public Level LogLevel { get; set; }

		public override string ToString() => new ToStringBuilder(nameof(CommandLineOptions))
		{
			{nameof(DockerContainer), DockerContainer},
			{nameof(SolutionDirectory), SolutionDirectory},
			{nameof(SharedWithDockerDirectory), SharedWithDockerDirectory},
			{nameof(LimitToTestProject), LimitToTestProject},
			{nameof(LimitToTestFilter), LimitToTestFilter},
			{nameof(DotnetTestVerbosity), DotnetTestVerbosity},
			{nameof(LogLevel), LogLevel},
			{nameof(EnvVarsFile), EnvVarsFile},
		}.ToString();
	}
}

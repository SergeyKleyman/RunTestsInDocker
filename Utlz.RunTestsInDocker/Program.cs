using System;
using System.Globalization;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using Utlz.Logging;
using Utlz.Logging.Serilog;
using Utlz.RunTestsInDocker.Logging;
using Utlz.RunTestsInDocker.Logging.Test;
using Logger = Utlz.Logging.Logger;
using SerilogILogger = Serilog.ILogger;
using SerilogLevel = Serilog.Events.LogEventLevel;
using UtlzLevel = Utlz.Logging.Level;

namespace Utlz.RunTestsInDocker
{
	internal class Program
	{
		internal const UtlzLevel DefaultLogLevel = UtlzLevel.Trace;

		private static Logger _logger;

		private static readonly CustomFormatProvider CustomFormatProviderSingleton = new CustomFormatProvider();

		/// <param name="isTestMode">Is in testing mode</param>
		// ReSharper disable once UnusedMember.Local
		private static int Main(string[] args)
		{
			SetupLogging(DefaultLogLevel);

			return Parser.Default.ParseArguments<CommandLineOptions>(args)
				.MapResult(options =>
					{
						LoggerSingleton.LevelController.Level = options.LogLevel;
						return new Impl(options).Run();
					},
					err => 1);
		}

		private static void DummyTestMethod() => _logger.Debug?.Log($"Message from {nameof(DummyTestMethod)}");

		private static void SetupLogging(UtlzLevel level)
		{
			// ReSharper disable once UseDeconstruction
			var serilogObjects = SetupSerilog(LevelConverter.Convert(level));

			LoggerSingleton.Setup(
				new LoggerFactory(new BackendLoggerFactory(serilogObjects.serilogger)),
				new LevelController(serilogObjects.levelSwitch));

			_logger = LoggerSingleton.Factory.GetLogger<Program>();
		}

		private static (SerilogILogger serilogger, LoggingLevelSwitch levelSwitch) SetupSerilog(SerilogLevel level)
		{
			var levelSwitch = new LoggingLevelSwitch(level);

			var serilogger =
				new LoggerConfiguration()
					.MinimumLevel.ControlledBy(levelSwitch)
					.Enrich.With<AllCapsLevelEnricher>()
					.WriteTo.Console(
						formatProvider: CustomFormatProviderSingleton,
						outputTemplate: BuildConsoleTemplate(),
						theme: ConsoleTheme.None)
					.CreateLogger();

			return (serilogger, levelSwitch);
		}

		private static string BuildConsoleTemplate()
		{
			// https://stackoverflow.com/questions/29470863/serilog-output-enrich-all-messages-with-methodname-from-which-log-entry-was-ca
			const string timestamp = "{Timestamp:HH:mm:ss.fff}";
			const string level = "{" + AllCapsLevelEnricher.PropertyName + ",-5}"; // pad to 5 on the right
			const string className = "{" + PropertyNames.SourceClassFullName + "}";
			const string methodName = "{" + PropertyNames.SourceMethodName + "}";
			const string message = "{Message:l}";
			var sourceLocation = $"{{{PropertyNames.SourceFilePath}}}:{{{PropertyNames.SourceLineNumber}}}";
			const string exception = "{NewLine}{Exception}";
			return $"{timestamp} | {level} | {className} | {methodName} | {message} | {sourceLocation}{exception}";
		}

		private static void TestLogging()
		{
			new TestClassWithStaticLogger(1234).LogVariousTestEvents();
			new TestClassWithLoggerPerInstance(LoggerSingleton.Factory, 98765).LogVariousTestEvents();
			DummyTestMethod();
		}

		private class CustomFormatProvider : IFormatProvider
		{
			private static readonly CustomFormatter CustomFormatterSingleton = new CustomFormatter();

			public object GetFormat(Type formatType) => typeof(ICustomFormatter).IsAssignableFrom(formatType)
				? CustomFormatterSingleton
				: null;

			private class CustomFormatter : ICustomFormatter
			{
				public string Format(string format, object arg, IFormatProvider formatProvider)
				{
					switch (arg)
					{
						case null:
							return "null";
						case string str:
							return $"`{str}'";
						case IFormattable formattable:
							return formattable.ToString(format, CultureInfo.InvariantCulture);
						default:
							return arg.ToString();
					}
				}
			}
		}
	}
}

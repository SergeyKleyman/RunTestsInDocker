using System;
using System.Text;
using ISerilogger = Serilog.ILogger;
using SerilogLevel = Serilog.Events.LogEventLevel;
using UtlzLevel = Utlz.Logging.Level;

namespace Utlz.Logging.Serilog
{
	public class BackendLogger : IBackendLogger
	{
		private readonly ISerilogger _serilogger;

		public BackendLogger(ISerilogger serilogger) => _serilogger = serilogger;

		public bool IsEnabled(Level level) =>
			_serilogger.IsEnabled(LevelConverter.Convert(level));

		public void Log(SourceContext sourceContext, StructuredMessage structuredMessage)
		{
			var loggerWithSourceContext = _serilogger
				.ForContext(PropertyNames.SourceMethodName, sourceContext.MemberName)
				.ForContext(PropertyNames.SourceFilePath, sourceContext.SourceFilePath)
				.ForContext(PropertyNames.SourceLineNumber, sourceContext.SourceLineNumber);

			var templateWithArgNamesOnly = structuredMessage.GetTemplateWithArgNamesOnly();
			var arguments = structuredMessage.Arguments;

			if (structuredMessage.AdditionalContext.Length != 0)
			{
				var newTemplate = new StringBuilder(templateWithArgNamesOnly);
				var newArguments = new object[structuredMessage.Arguments.Length +
				                              structuredMessage.AdditionalContext.Length];
				Array.Copy(arguments, 0, newArguments, 0, arguments.Length);
				var nextIndex = arguments.Length;
				newTemplate.Append(" [Additional context: ");
				foreach (var nameValuePair in structuredMessage.AdditionalContext)
				{
					if (nextIndex != arguments.Length) newTemplate.Append("; ");
					newTemplate.Append($"{nameValuePair.Item1}: {{{nameValuePair.Item1}}}");
					newArguments[nextIndex++] = nameValuePair.Item2;
				}

				newTemplate.Append("]");
				templateWithArgNamesOnly = newTemplate.ToString();
				arguments = newArguments;
			}

			loggerWithSourceContext.Write(LevelConverter.Convert(sourceContext.Level), structuredMessage.Exception,
				templateWithArgNamesOnly, arguments);
		}
	}
}

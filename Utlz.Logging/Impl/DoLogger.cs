namespace Utlz.Logging.Impl
{
	internal struct DoLogger
	{
		private readonly IBackendLogger _backendLogger;

		internal DoLogger(IBackendLogger backendLogger) => _backendLogger = backendLogger;

		internal bool IsEnabled(Level level) => _backendLogger.IsEnabled(level);

		internal void DoLog(SourceContext sourceContext, StructuredMessage structuredMessage) =>
			_backendLogger.Log(sourceContext, structuredMessage);
	}
}

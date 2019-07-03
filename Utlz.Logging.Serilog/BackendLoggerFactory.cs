using ISerilogger = Serilog.ILogger;

namespace Utlz.Logging.Serilog
{
	public class BackendLoggerFactory : IBackendLoggerFactory
	{
		private readonly ISerilogger _serilogger;

		public BackendLoggerFactory(ISerilogger serilogger) => _serilogger = serilogger;

		public IBackendLogger GetLogger(string fullClassName) =>
			new BackendLogger(_serilogger.ForContext(PropertyNames.SourceClassFullName, fullClassName));
	}
}

using Utlz.Logging;

namespace Utlz.RunTestsInDocker.Logging.Test
{
	internal class TestClassWithLoggerPerInstance: TestClassWithLoggerBase
	{
		private readonly Logger _logger;

		internal TestClassWithLoggerPerInstance(LoggerFactory loggerFactory, int instanceId)
		{
			_logger = loggerFactory.GetLogger(GetType().FullName + $".#{instanceId}");

			_logger.Trace?.Log($"Created new instance with {instanceId:InstanceId}");
		}

		internal void LogVariousTestEvents() => LogVariousTestEvents(_logger);
	}
}

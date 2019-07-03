using Utlz.Logging;

namespace Utlz.RunTestsInDocker.Logging.Test
{
	internal class TestClassWithStaticLogger: TestClassWithLoggerBase
	{
		private static readonly Logger Logger = LoggerSingleton.Factory.GetLogger<TestClassWithStaticLogger>();

		internal TestClassWithStaticLogger(int instanceId)
		{
			Logger.Trace?.Log($"Created new instance with {instanceId:InstanceId}");
		}

		internal void LogVariousTestEvents() => LogVariousTestEvents(Logger);
	}
}

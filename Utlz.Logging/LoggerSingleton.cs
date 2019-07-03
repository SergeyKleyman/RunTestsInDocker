using Utlz.Logging;

namespace Utlz.RunTestsInDocker
{
	internal static class LoggerSingleton
	{
		internal static LoggerFactory Factory { get; private set; }

		internal static ILevelController LevelController { get; private set; }

		internal static void Setup(LoggerFactory factory, ILevelController levelController)
		{
			Factory = factory;
			LevelController = levelController;
		}
	}
}

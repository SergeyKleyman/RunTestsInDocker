using System;
using Utlz.Logging;

namespace Utlz.RunTestsInDocker.Logging.Test
{
	internal abstract class TestClassWithLoggerBase
	{
		protected void LogVariousTestEvents(Logger logger)
		{
			logger.Trace?.Log($"Message for static level {Level.Trace:LogLevel}");
			logger.Debug?.Log($"Message for static level {Level.Debug:LogLevel}");
			logger.Info?.Log($"Message for static level {Level.Info:LogLevel}");
			logger.Warn?.Log($"Message for static level {Level.Warn:LogLevel}");
			logger.Error?.Log($"Message for static level {Level.Error:LogLevel}");
			logger.Fatal?.Log($"Message for static level {Level.Fatal:LogLevel}");

			foreach (Level level in Enum.GetValues(typeof(Level)))
			{
				logger.ForLevel(level)?.Log($"Message for dynamic level {level:LogLevel}");
			}

			logger.Trace?.Log("Text only message via static level");
			logger.Debug?.Log("Text only message via static level");
			logger.Info?.Log("Text only message via static level");
			logger.Warn?.Log("Text only message via static level");
			logger.Error?.Log("Text only message via static level");
			logger.Fatal?.Log("Text only message via static level");

			foreach (Level level in Enum.GetValues(typeof(Level)))
			{
				logger.ForLevel(level)?.Log($"Text only message via dynamic level");
			}
		}
	}
}

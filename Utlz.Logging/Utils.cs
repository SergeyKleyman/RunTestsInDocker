namespace Utlz.Logging
{
	public static class Utils
	{
		public static string GetDisplayName(this Level level)
		{
			switch (level)
			{
				case Level.Trace: return "TRACE";
				case Level.Debug: return "DEBUG";
				case Level.Info: return "INFO";
				case Level.Warn: return "WARN";
				case Level.Error: return "ERROR";
				case Level.Fatal: return "FATAL";
				default: return null;
			}
		}
	}
}

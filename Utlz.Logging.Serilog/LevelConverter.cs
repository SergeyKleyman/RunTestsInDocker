using SerilogLevel = Serilog.Events.LogEventLevel;
using UtlzLevel = Utlz.Logging.Level;

namespace Utlz.Logging.Serilog
{
	public static class LevelConverter
	{
		public static SerilogLevel Convert(UtlzLevel level)
		{
			switch (level)
			{
				case UtlzLevel.Fatal: return SerilogLevel.Fatal;
				case UtlzLevel.Error: return SerilogLevel.Error;
				case UtlzLevel.Warn: return SerilogLevel.Warning;
				case UtlzLevel.Info: return SerilogLevel.Information;
				case UtlzLevel.Debug: return SerilogLevel.Debug;
				case UtlzLevel.Trace: return SerilogLevel.Verbose;
				default: return SerilogLevel.Information;
			}
		}

		public static UtlzLevel Convert(SerilogLevel level)
		{
			switch (level)
			{
				case SerilogLevel.Fatal: return UtlzLevel.Fatal;
				case SerilogLevel.Error: return UtlzLevel.Error;
				case SerilogLevel.Warning: return UtlzLevel.Warn;
				case SerilogLevel.Information: return UtlzLevel.Info;
				case SerilogLevel.Debug: return UtlzLevel.Debug;
				case SerilogLevel.Verbose: return UtlzLevel.Trace;
				default: return UtlzLevel.Info;
			}
		}
	}
}

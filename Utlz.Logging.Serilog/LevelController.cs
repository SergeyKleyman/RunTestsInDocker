using Serilog.Core;

namespace Utlz.Logging.Serilog
{
	public class LevelController : ILevelController
	{
		private readonly LoggingLevelSwitch _loggingLevelSwitch;

		public LevelController(LoggingLevelSwitch loggingLevelSwitch) => _loggingLevelSwitch = loggingLevelSwitch;

		public Level Level
		{
			get => LevelConverter.Convert(_loggingLevelSwitch.MinimumLevel);
			set => _loggingLevelSwitch.MinimumLevel = LevelConverter.Convert(value);
		}
	}
}

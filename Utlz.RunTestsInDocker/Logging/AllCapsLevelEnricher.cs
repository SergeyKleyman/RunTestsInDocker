using Serilog.Core;
using Serilog.Events;
using Utlz.Logging;
using Utlz.Logging.Serilog;

namespace Utlz.RunTestsInDocker.Logging
{
	internal class AllCapsLevelEnricher : ILogEventEnricher
	{
		internal const string PropertyName = "AllCapsLevel";

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var levelName = LevelConverter.Convert(logEvent.Level).GetDisplayName() ?? "UNKNOWN";
			logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(PropertyName, levelName));
		}
	}
}

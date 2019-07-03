namespace Utlz.Logging
{
    public interface IBackendLogger
    {
	    bool IsEnabled(Level level);
        void Log(SourceContext sourceContext, StructuredMessage structuredMessage);
    }
}

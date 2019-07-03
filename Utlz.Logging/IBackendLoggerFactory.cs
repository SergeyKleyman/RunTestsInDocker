namespace Utlz.Logging
{
	public interface IBackendLoggerFactory
	{
		IBackendLogger GetLogger(string fullClassName);
	}
}

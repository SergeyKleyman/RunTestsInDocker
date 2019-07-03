using Utlz.Logging.Impl;

namespace Utlz.Logging
{
    internal struct LoggerFactory
    {
        private readonly IBackendLoggerFactory _backendLoggerFactory;

        internal LoggerFactory(IBackendLoggerFactory backendLoggerFactory)
        {
            _backendLoggerFactory = backendLoggerFactory;
        }

        internal Logger GetLogger<TClass>()
        {
            return GetLogger(typeof(TClass).FullName);
        }

        internal Logger GetLogger(string fullClassName)
        {
            return new Logger(new DoLogger(_backendLoggerFactory.GetLogger(fullClassName)));
        }
    }
}
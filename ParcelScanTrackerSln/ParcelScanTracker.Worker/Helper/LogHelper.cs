namespace ParcelScanTracker.Worker
{
    /// <summary>
    /// Simple log service for writing logs with type and message.
    /// Wraps ILogger to provide typed logging for Info, Warning, Error, and Debug.
    /// </summary>
    public class LogHelper
    {
        private readonly ILogger<LogHelper> _logger;

        /// <summary>
        /// Initializes the LogHelper with an injected logger.
        /// </summary>
        public LogHelper(ILogger<LogHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Writes a log entry with the specified log type and message.
        /// Routes to the correct ILogger method based on LogType.
        /// </summary>
        public virtual void WriteToLog(LogType logType, string message)
        {
            // Log based on log type
            switch (logType)
            {
                case LogType.Info:
                    _logger.LogInformation(message);
                    break;
                case LogType.Warning:
                    _logger.LogWarning(message);
                    break;
                case LogType.Error:
                    _logger.LogError(message);
                    break;
                case LogType.Debug:
                    _logger.LogDebug(message);
                    break;
                default:
                    _logger.LogInformation(message);
                    break;
            }
        }
    }
}

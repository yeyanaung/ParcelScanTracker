// Tests LogHelper logging for each log type.
using Microsoft.Extensions.Logging;
using Moq;
using ParcelScanTracker.Worker;

public class LogHelperTests
{
    [Theory]
    [InlineData(LogType.Info)]
    [InlineData(LogType.Warning)]
    [InlineData(LogType.Error)]
    [InlineData(LogType.Debug)]
    public void WriteToLog_LogsCorrectLevel(LogType logType)
    {
        // Arrange: create logger mock and LogHelper
        var loggerMock = new Mock<ILogger<LogHelper>>();
        var logHelper = new LogHelper(loggerMock.Object);
        var message = "Test message";

        // Act: log the message
        logHelper.WriteToLog(logType, message);

        // Assert: correct log method called
        switch (logType)
        {
            case LogType.Info:
                loggerMock.Verify(l => l.LogInformation(message), Times.Once);
                break;
            case LogType.Warning:
                loggerMock.Verify(l => l.LogWarning(message), Times.Once);
                break;
            case LogType.Error:
                loggerMock.Verify(l => l.LogError(message), Times.Once);
                break;
            case LogType.Debug:
                loggerMock.Verify(l => l.LogDebug(message), Times.Once);
                break;
        }
    }
}

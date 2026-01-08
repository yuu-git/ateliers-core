using Ateliers.Logging;
using Xunit;

namespace Ateliers.Core.UnitTests.Logging;

/// <summary>
/// ConsoleLogger のテスト
/// </summary>
public class ConsoleLoggerTests
{
    [Fact(DisplayName = @"Log メソッドは例外をスローしない")]
    public void Log_ShouldNotThrow()
    {
        // Arrange
        var options = new LoggerOptions
        {
            MinimumLevel = LogLevel.Debug,
            Category = "Test"
        };
        var logger = new ConsoleLogger(options);

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            logger.Trace("Trace message");
            logger.Debug("Debug message");
            logger.Info("Info message");
            logger.Warn("Warn message");
            logger.Error("Error message");
            logger.Critical("Critical message");
        });

        Assert.Null(exception);
    }

    [Fact(DisplayName = @"例外を含むログは例外をスローしない")]
    public void Log_WithException_ShouldNotThrow()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger = new ConsoleLogger(options);
        var testException = new InvalidOperationException("Test exception");

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            logger.Error("Error occurred", testException);
        });

        Assert.Null(exception);
    }

    [Fact(DisplayName = @"カテゴリを含むログは例外をスローしない")]
    public void Log_WithCategory_ShouldNotThrow()
    {
        // Arrange
        var options = new LoggerOptions
        {
            MinimumLevel = LogLevel.Debug,
            Category = "TestCategory"
        };
        var logger = new ConsoleLogger(options);

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            logger.Info("Test message");
        });

        Assert.Null(exception);
    }
}

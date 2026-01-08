using Ateliers.Logging;
using Xunit;

namespace Ateliers.Core.UnitTests.Logging;

/// <summary>
/// CompositeLogger のテスト
/// </summary>
public class CompositeLoggerTests
{
    [Fact(DisplayName = @"Log メソッドはすべてのロガーに転送する")]
    public void Log_ShouldForwardToAllLoggers()
    {
        // Arrange
        var options1 = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger1 = new InMemoryLogger(options1);

        var options2 = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger2 = new InMemoryLogger(options2);

        var composite = new CompositeLogger(new ILogger[] { logger1, logger2 });

        // Act
        composite.Info("Test message");

        // Assert
        Assert.Single(logger1.Entries);
        Assert.Single(logger2.Entries);
        Assert.Equal("Test message", logger1.Entries[0].Message);
        Assert.Equal("Test message", logger2.Entries[0].Message);
    }

    [Fact(DisplayName = @"失敗するロガーを含むログは例外をスローしない")]
    public void Log_WithFailingLogger_ShouldNotThrow()
    {
        // Arrange
        var options1 = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger1 = new InMemoryLogger(options1);

        var failingLogger = new FailingLogger();

        var composite = new CompositeLogger(new ILogger[] { logger1, failingLogger });

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            composite.Info("Test message");
        });

        Assert.Null(exception);
        Assert.Single(logger1.Entries);
    }

    [Fact(DisplayName = @"コンストラクタは null のロガーで例外をスローする")]
    public void Constructor_WithNullLoggers_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new CompositeLogger(null!);
        });
    }

    [Fact(DisplayName = @"カテゴリを含むログはカテゴリを転送する")]
    public void Log_WithCategory_ShouldForwardCategory()
    {
        // Arrange
        var options1 = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger1 = new InMemoryLogger(options1);

        var options2 = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger2 = new InMemoryLogger(options2);

        var composite = new CompositeLogger(new ILogger[] { logger1, logger2 }, "TestCategory");

        // Act
        composite.Info("Test message");

        // Assert
        Assert.Equal("TestCategory", logger1.Entries[0].Category);
        Assert.Equal("TestCategory", logger2.Entries[0].Category);
    }

    private class FailingLogger : LoggerBase
    {
        public FailingLogger() : base(null) { }

        public override void Log(LogEntry entry)
        {
            throw new InvalidOperationException("Simulated failure");
        }
    }
}

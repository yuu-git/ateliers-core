using Ateliers.Logging;
using Xunit;

namespace Ateliers.Core.UnitTests.Logging;

/// <summary>
/// InMemoryLogger のテスト
/// </summary>
public class InMemoryLoggerTests
{
    [Fact(DisplayName = @"ログエントリが保存されること")]
    public void Log_ShouldStoreEntry()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger = new InMemoryLogger(options);

        // Act
        logger.Info("Test message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal("Test message", logger.Entries[0].Message);
        Assert.Equal(LogLevel.Information, logger.Entries[0].Level);
    }

    [Fact(DisplayName = @"最小ログレベルが尊重されること")]
    public void Log_ShouldRespectMinimumLevel()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Information };
        var logger = new InMemoryLogger(options);

        // Act
        logger.Debug("Debug message");
        logger.Info("Info message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal("Info message", logger.Entries[0].Message);
    }

    [Fact(DisplayName = @"カテゴリを含むログはカテゴリを保存すること")]
    public void Log_WithCategory_ShouldStoreCategory()
    {
        // Arrange
        var options = new LoggerOptions
        {
            MinimumLevel = LogLevel.Debug,
            Category = "TestCategory"
        };
        var logger = new InMemoryLogger(options);

        // Act
        logger.Info("Test message");

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal("TestCategory", logger.Entries[0].Category);
    }

    [Fact(DisplayName = @"Clear メソッドはすべてのエントリを削除すること")]
    public void Clear_ShouldRemoveAllEntries()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger = new InMemoryLogger(options);
        logger.Info("Message 1");
        logger.Info("Message 2");

        // Act
        logger.Clear();

        // Assert
        Assert.Empty(logger.Entries);
    }

    [Fact(DisplayName = @"すべてのログレベルが動作すること")]
    public void Log_AllLevels_ShouldWork()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Trace };
        var logger = new InMemoryLogger(options);

        // Act
        logger.Trace("Trace");
        logger.Debug("Debug");
        logger.Info("Info");
        logger.Warn("Warn");
        logger.Error("Error");
        logger.Critical("Critical");

        // Assert
        Assert.Equal(6, logger.Entries.Count);
        Assert.Equal(LogLevel.Trace, logger.Entries[0].Level);
        Assert.Equal(LogLevel.Debug, logger.Entries[1].Level);
        Assert.Equal(LogLevel.Information, logger.Entries[2].Level);
        Assert.Equal(LogLevel.Warning, logger.Entries[3].Level);
        Assert.Equal(LogLevel.Error, logger.Entries[4].Level);
        Assert.Equal(LogLevel.Critical, logger.Entries[5].Level);
    }

    [Fact(DisplayName = @"例外を含むログは例外を保存すること")]
    public void Log_WithException_ShouldStoreException()
    {
        // Arrange
        var options = new LoggerOptions { MinimumLevel = LogLevel.Debug };
        var logger = new InMemoryLogger(options);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.Error("Error occurred", exception);

        // Assert
        Assert.Single(logger.Entries);
        Assert.Equal(exception, logger.Entries[0].Exception);
    }
}

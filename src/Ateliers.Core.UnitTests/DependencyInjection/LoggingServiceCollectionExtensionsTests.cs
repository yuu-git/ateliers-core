using Ateliers.DependencyInjection;
using Ateliers.Logging;
using Ateliers.Logging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Core.UnitTests.DependencyInjection;

/// <summary>
/// LoggingServiceCollectionExtensions のテスト
/// </summary>
public class LoggingServiceCollectionExtensionsTests
{
    [Fact(DisplayName = @"AddAteliersLogging はロガーを登録すること")]
    public void AddAteliersLogging_ShouldRegisterLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAteliersLogging();
        var provider = services.BuildServiceProvider();
        var logger = provider.GetService<ILogger>();

        // Assert
        Assert.NotNull(logger);
    }

    [Fact(DisplayName = @"AddAteliersLogging は設定を適用すること")]
    public void AddAteliersLogging_WithConfiguration_ShouldApplyConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .SetCategory("Test")
                .SetMinimumLevel(LogLevel.Debug)
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger>();

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(memoryLogger);
        logger.Debug("Test message");
        Assert.Single(memoryLogger.Entries);
        Assert.Equal("Test", memoryLogger.Entries[0].Category);
    }

    [Fact(DisplayName = @"AddAteliersLogging は設定なしでデフォルトのコンソールロガーを使用すること")]
    public void AddAteliersLogging_WithoutConfiguration_ShouldUseDefaultConsoleLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAteliersLogging();
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger>();

        // Assert
        Assert.NotNull(logger);
        // コンソールロガーがデフォルトで追加されることを確認
        var exception = Record.Exception(() => logger.Info("Test message"));
        Assert.Null(exception);
    }

    [Fact(DisplayName = @"AddAteliersLogging は複数のロガーでコンポジットロガーを作成すること")]
    public void AddAteliersLogging_WithMultipleLoggers_ShouldCreateCompositeLogger()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .AddConsole()
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger>();

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(memoryLogger);
        logger.Info("Test message");
        Assert.Single(memoryLogger.Entries);
    }

    [Fact(DisplayName = @"AddAteliersLogging はカスタムロガーを登録すること")]
    public void AddAteliersLogging_WithCustomLogger_ShouldRegisterCustomLogger()
    {
        // Arrange
        var services = new ServiceCollection();
        var customLogger = new InMemoryLogger(new LoggerOptions());

        // Act
        services.AddAteliersLogging(logging =>
        {
            logging.AddLogger(customLogger);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger>();

        // Assert
        Assert.NotNull(logger);
        logger.Info("Test message");
        Assert.Single(customLogger.Entries);
    }
}

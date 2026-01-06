using Ateliers.DependencyInjection;
using Ateliers.Logging;
using Ateliers.Logging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Core.UnitTests.Integration;

/// <summary>
/// ロギングと実行コンテキストの統合テスト
/// </summary>
public class LoggingWithExecutionContextTests
{
    [Fact]
    public void LoggingWithContext_ShouldIncludeCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();

        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .SetCategory("Test")
                .SetMinimumLevel(LogLevel.Debug)
                .AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        using (var contextScope = context.BeginScope("TestOperation"))
        {
            logger.Info("Test message");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Single(memoryLogger.Entries);
        Assert.NotNull(memoryLogger.Entries[0].CorrelationId);
        Assert.Equal("Test", memoryLogger.Entries[0].Category);
        Assert.Equal("Test message", memoryLogger.Entries[0].Message);
    }

    [Fact]
    public void NestedScopes_ShouldHaveDifferentCorrelationIds()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();

        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        using (var scope1 = context.BeginScope("Outer"))
        {
            logger.Info("Outer message");

            using (var scope2 = context.BeginScope("Inner"))
            {
                logger.Info("Inner message");
            }

            logger.Info("Outer message 2");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Equal(3, memoryLogger.Entries.Count);

        var outerCorrelationId = memoryLogger.Entries[0].CorrelationId;
        var innerCorrelationId = memoryLogger.Entries[1].CorrelationId;

        Assert.NotEqual(outerCorrelationId, innerCorrelationId);
        Assert.Equal(outerCorrelationId, memoryLogger.Entries[2].CorrelationId);
    }

    [Fact]
    public async Task AsyncOperations_ShouldMaintainCorrelationId()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();

        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging.AddInMemory(out memoryLogger);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        using (var contextScope = context.BeginScope("AsyncOperation"))
        {
            logger.Info("Before delay");
            await Task.Delay(10);
            logger.Info("After delay");
        }

        // Assert
        Assert.NotNull(memoryLogger);
        Assert.Equal(2, memoryLogger.Entries.Count);
        Assert.Equal(
            memoryLogger.Entries[0].CorrelationId,
            memoryLogger.Entries[1].CorrelationId);
    }

    [Fact]
    public void MultipleLoggers_WithContext_ShouldAllReceiveLogs()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();

        InMemoryLogger memoryLogger1 = null!;
        InMemoryLogger memoryLogger2 = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .SetCategory("Test")
                .AddInMemory(out memoryLogger1)
                .AddInMemory(out memoryLogger2);
        });

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        using (var contextScope = context.BeginScope("TestOperation"))
        {
            logger.Info("Test message");
        }

        // Assert
        Assert.NotNull(memoryLogger1);
        Assert.NotNull(memoryLogger2);
        Assert.Single(memoryLogger1.Entries);
        Assert.Single(memoryLogger2.Entries);
        Assert.Equal(
            memoryLogger1.Entries[0].CorrelationId,
            memoryLogger2.Entries[0].CorrelationId);
    }
}

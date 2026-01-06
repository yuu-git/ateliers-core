using Ateliers.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ateliers.Core.UnitTests.DependencyInjection;

/// <summary>
/// ExecutionContextServiceCollectionExtensions のテスト
/// </summary>
public class ExecutionContextServiceCollectionExtensionsTests
{
    [Fact]
    public void AddAteliersExecutionContext_ShouldRegisterContext()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetService<IExecutionContext>();

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void AddAteliersExecutionContext_ShouldCreateNewContextWhenNoCurrentContext()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.CorrelationId);
    }

    [Fact]
    public void AddAteliersExecutionContext_ShouldBeScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();

        // Act
        IExecutionContext? context1;
        IExecutionContext? context2;

        using (var scope1 = provider.CreateScope())
        {
            context1 = scope1.ServiceProvider.GetRequiredService<IExecutionContext>();
        }

        using (var scope2 = provider.CreateScope())
        {
            context2 = scope2.ServiceProvider.GetRequiredService<IExecutionContext>();
        }

        // Assert
        Assert.NotEqual(context1.CorrelationId, context2.CorrelationId);
    }
}

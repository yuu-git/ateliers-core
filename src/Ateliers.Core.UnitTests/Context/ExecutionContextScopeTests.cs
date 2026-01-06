using Ateliers.Context;
using Xunit;

namespace Ateliers.Core.UnitTests.Context;

/// <summary>
/// ExecutionContextScope のテスト
/// </summary>
public class ExecutionContextScopeTests
{
    [Fact]
    public void Constructor_ShouldCreateNewContext()
    {
        // Arrange & Act
        using var scope = new ExecutionContextScope("TestProperties");
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.NotNull(current.CorrelationId);
        Assert.Equal("TestProperties", current.Properties);
    }

    [Fact]
    public void Dispose_ShouldRestorePreviousContext()
    {
        // Arrange
        Ateliers.Context.ExecutionContext? contextBeforeScope = null;
        Ateliers.Context.ExecutionContext? contextDuringScope = null;
        Ateliers.Context.ExecutionContext? contextAfterScope = null;

        // Act
        contextBeforeScope = Ateliers.Context.ExecutionContext.Current;

        using (var scope = new ExecutionContextScope("TestProperties"))
        {
            contextDuringScope = Ateliers.Context.ExecutionContext.Current;
        }

        contextAfterScope = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.Null(contextBeforeScope);
        Assert.NotNull(contextDuringScope);
        Assert.Null(contextAfterScope);
    }

    [Fact]
    public void Nested_Scopes_ShouldMaintainHierarchy()
    {
        // Arrange & Act
        string? correlationId1 = null;
        string? correlationId2 = null;
        string? correlationIdAfterInner = null;

        using (var outerScope = new ExecutionContextScope("Outer"))
        {
            correlationId1 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;

            using (var innerScope = new ExecutionContextScope("Inner"))
            {
                correlationId2 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
                Assert.NotEqual(correlationId1, correlationId2);
            }

            correlationIdAfterInner = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        // Assert
        Assert.NotNull(correlationId1);
        Assert.NotNull(correlationId2);
        Assert.Equal(correlationId1, correlationIdAfterInner);
    }

    [Fact]
    public async Task Async_Operations_ShouldMaintainContext()
    {
        // Arrange & Act
        string? correlationId1 = null;
        string? correlationId2 = null;

        using (var scope = new ExecutionContextScope("AsyncTest"))
        {
            correlationId1 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
            await Task.Delay(10);
            correlationId2 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        // Assert
        Assert.Equal(correlationId1, correlationId2);
    }

    [Fact]
    public void Constructor_WithNullProperties_ShouldWork()
    {
        // Arrange & Act
        using var scope = new ExecutionContextScope(null);
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Null(current.Properties);
    }

    [Fact]
    public void Multiple_Sequential_Scopes_ShouldHaveDifferentCorrelationIds()
    {
        // Arrange & Act
        string? correlationId1;
        string? correlationId2;

        using (var scope1 = new ExecutionContextScope("Scope1"))
        {
            correlationId1 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        using (var scope2 = new ExecutionContextScope("Scope2"))
        {
            correlationId2 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        // Assert
        Assert.NotEqual(correlationId1, correlationId2);
    }
}

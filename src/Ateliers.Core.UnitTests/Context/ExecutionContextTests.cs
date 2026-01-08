using Xunit;

namespace Ateliers.Core.UnitTests.Context;

/// <summary>
/// ExecutionContext のテスト
/// </summary>
public class ExecutionContextTests
{
    [Fact(DisplayName = @"コンストラクタはプロパティを設定する")]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange & Act
        var context = new Ateliers.Context.ExecutionContext("test-correlation-id", "TestProperties");

        // Assert
        Assert.Equal("test-correlation-id", context.CorrelationId);
        Assert.Equal("TestProperties", context.Properties);
    }

    [Fact(DisplayName = @"Current はスコープが作成されていない場合は null")]
    public void Current_ShouldBeNull_WhenNoScopeCreated()
    {
        // Act
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.Null(current);
    }

    [Fact(DisplayName = @"BeginScope は Current を設定する")]
    public void BeginScope_ShouldSetCurrent()
    {
        // Arrange
        var context = new Ateliers.Context.ExecutionContext("test-correlation-id", "TestProperties");

        // Act
        using var scope = context.BeginScope("ScopeProperties");
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.NotEqual(context.CorrelationId, current.CorrelationId);
        Assert.Equal("ScopeProperties", current.Properties);
    }

    [Fact(DisplayName = @"BeginScope の Dispose は以前のコンテキストを復元する")]
    public void BeginScope_Dispose_ShouldRestorePreviousContext()
    {
        // Arrange
        var context = new Ateliers.Context.ExecutionContext("test-correlation-id", "TestProperties");

        // Act
        string? scopeCorrelationId;
        using (var scope = context.BeginScope("ScopeProperties"))
        {
            scopeCorrelationId = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        var currentAfterDispose = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(scopeCorrelationId);
        Assert.Null(currentAfterDispose);
    }

    [Fact(DisplayName = @"BeginScope のネストは階層を作成する")]
    public void BeginScope_Nested_ShouldCreateHierarchy()
    {
        // Arrange
        var context = new Ateliers.Context.ExecutionContext("root-id", "RootProperties");

        // Act & Assert
        using (var scope1 = context.BeginScope("Scope1"))
        {
            var current1 = Ateliers.Context.ExecutionContext.Current;
            Assert.NotNull(current1);
            Assert.Equal("Scope1", current1.Properties);

            using (var scope2 = context.BeginScope("Scope2"))
            {
                var current2 = Ateliers.Context.ExecutionContext.Current;
                Assert.NotNull(current2);
                Assert.Equal("Scope2", current2.Properties);
                Assert.NotEqual(current1.CorrelationId, current2.CorrelationId);
            }

            var currentAfterScope2 = Ateliers.Context.ExecutionContext.Current;
            Assert.NotNull(currentAfterScope2);
            Assert.Equal(current1.CorrelationId, currentAfterScope2.CorrelationId);
        }
    }

    [Fact(DisplayName = @"BeginScope の非同期操作はコンテキストを維持する")]
    public async Task BeginScope_Async_ShouldMaintainContext()
    {
        // Arrange
        var context = new Ateliers.Context.ExecutionContext("test-id", "TestProperties");

        // Act
        string? correlationId1 = null;
        string? correlationId2 = null;

        using (var scope = context.BeginScope("AsyncScope"))
        {
            correlationId1 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
            await Task.Delay(10);
            correlationId2 = Ateliers.Context.ExecutionContext.Current?.CorrelationId;
        }

        // Assert
        Assert.NotNull(correlationId1);
        Assert.Equal(correlationId1, correlationId2);
    }

    [Fact(DisplayName = @"BeginScope は null のプロパティで動作する")]
    public void BeginScope_WithNullProperties_ShouldWork()
    {
        // Arrange
        var context = new Ateliers.Context.ExecutionContext("test-id", "TestProperties");

        // Act
        using var scope = context.BeginScope(null);
        var current = Ateliers.Context.ExecutionContext.Current;

        // Assert
        Assert.NotNull(current);
        Assert.Null(current.Properties);
    }
}

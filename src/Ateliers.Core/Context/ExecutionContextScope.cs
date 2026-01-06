namespace Ateliers.Context;

/// <summary>
/// 実行コンテキスト スコープを表します。
/// </summary>
public class ExecutionContextScope : IDisposable
{
    private readonly ExecutionContext? _previous;

    /// <summary>
    /// 実行コンテキスト スコープ の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="operationName">操作名</param>
    /// <param name="correlationId">相関 ID（省略時は自動生成）</param>
    public ExecutionContextScope(string operationName, string? correlationId = null)
    {
        _previous = ExecutionContext.Current;

        var newContext = CreateContext(
            correlationId ?? Guid.NewGuid().ToString("N"),
            operationName
        );

        SetContext(newContext);
    }

    /// <summary>
    /// 新しいコンテキストを作成します。
    /// </summary>
    /// <param name="correlationId">相関 ID</param>
    /// <param name="operationName">操作名</param>
    /// <returns>作成されたコンテキスト</returns>
    protected virtual ExecutionContext CreateContext(string correlationId, string operationName)
    {
        return new ExecutionContext(correlationId, operationName);
    }

    /// <summary>
    /// コンテキストを設定します。
    /// </summary>
    /// <param name="context">設定するコンテキスト</param>
    protected void SetContext(ExecutionContext? context)
    {
        typeof(ExecutionContext)
            .GetMethod("SetCurrent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.Invoke(null, [context]);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        SetContext(_previous);
        GC.SuppressFinalize(this);
    }
}

using System.Threading;

namespace Ateliers.Context;

/// <summary>
/// 実行コンテキストを表します。
/// </summary>
public class ExecutionContext : IExecutionContext
{
    private static readonly AsyncLocal<ExecutionContext> _current = new();

    /// <summary>
    /// 現在の実行コンテキストを取得します。
    /// </summary>
    public static ExecutionContext? Current => _current.Value;

    /// <summary>
    /// 相関 ID を取得します。
    /// </summary>
    public string CorrelationId { get; init; }

    /// <summary>
    /// プロパティを取得します。
    /// </summary>
    public string? Properties { get; init; }

    /// <summary>
    /// 実行コンテキスト の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="correlationId">相関 ID</param>
    /// <param name="properties">プロパティ</param>
    public ExecutionContext(string correlationId, string? properties)
    {
        CorrelationId = correlationId;
        Properties = properties;
    }

    /// <summary>
    /// 新しい実行コンテキストスコープを開始します。
    /// </summary>
    /// <param name="properties">プロパティ</param>
    /// <returns>実行コンテキストスコープ</returns>
    public virtual ExecutionContextScope BeginScope(string? properties = null)
    {
        return new ExecutionContextScope(properties);
    }

    /// <summary>
    /// 現在の実行コンテキストを設定します。
    /// </summary>
    /// <param name="context">実行コンテキスト</param>
    internal static void SetCurrent(ExecutionContext? context)
    {
        _current.Value = context;
    }
}

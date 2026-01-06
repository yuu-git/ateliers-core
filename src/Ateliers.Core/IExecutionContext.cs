using Ateliers.Context;

namespace Ateliers;

/// <summary>
/// 実行コンテキストを管理するインターフェイス
/// </summary>
public interface IExecutionContext
{
    /// <summary>
    /// 相関IDを取得します。
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// プロパティを取得します。
    /// </summary>
    string? Properties { get; }

    /// <summary>
    /// 新しい実行コンテキストスコープを開始します。
    /// </summary>
    /// <param name="properties">プロパティ</param>
    /// <returns>実行コンテキストスコープ</returns>
    ExecutionContextScope BeginScope(string? properties = null);
}

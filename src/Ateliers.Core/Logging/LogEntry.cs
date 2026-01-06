namespace Ateliers.Logging;

/// <summary>
/// ログ エントリを表します。
/// </summary>
public class LogEntry
{
    /// <summary>
    /// ログエントリのタイムスタンプを取得します。
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    /// <summary>
    /// ログレベルを取得します。
    /// </summary>
    public LogLevel Level { get; init; }

    /// <summary>
    /// ログの原文を取得します。
    /// </summary>
    public string LogText { get; init; } = string.Empty;

    /// <summary>
    /// ログメッセージを取得します。
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// 関連する例外を取得します。
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// 相関 ID を取得します。
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// カテゴリを取得します（例: "MCP", "VoiceEngine", "App"）。
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// 追加のプロパティを取得します。
    /// </summary>
    public Dictionary<string, object>? Properties { get; init; }
}

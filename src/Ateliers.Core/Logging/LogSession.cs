namespace Ateliers.Logging;

/// <summary>
/// ログ セッションを表します。
/// </summary>
public class LogSession
{
    /// <summary>
    /// 相関 ID
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// ログ エントリのコレクション
    /// </summary>
    public IReadOnlyList<LogEntry> Entries { get; init; }
        = Array.Empty<LogEntry>();
}

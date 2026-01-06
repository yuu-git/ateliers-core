using Ateliers.Context;

namespace Ateliers.Logging;

/// <summary>
/// LogEntry を作成するファクトリ
/// </summary>
internal static class LogEntryFactory
{
    /// <summary>
    /// 新しい LogEntry を作成します。
    /// </summary>
    /// <param name="level">ログレベル</param>
    /// <param name="message">ログメッセージ</param>
    /// <param name="exception">例外（オプション）</param>
    /// <param name="category">ログのカテゴリ（オプション）</param>
    /// <returns>作成された LogEntry</returns>
    public static LogEntry Create(LogLevel level, string message, Exception? exception = null, string? category = null)
    {
        var ctx = Context.ExecutionContext.Current;

        return new LogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Level = level,
            LogText = message,
            Message = message,
            Exception = exception,
            CorrelationId = ctx?.CorrelationId,
            Category = category
        };
    }
}

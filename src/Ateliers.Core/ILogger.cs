using Ateliers.Logging;

namespace Ateliers;

/// <summary>
/// ロガーを表します。
/// </summary>
public interface ILogger
{
    /// <summary>
    /// ログエントリを記録します。
    /// </summary>
    /// <param name="entry">ログエントリ</param>
    void Log(LogEntry entry);

    /// <summary>
    /// トレース レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    void Trace(string message);

    /// <summary>
    /// デバッグ レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    void Debug(string message);

    /// <summary>
    /// 情報レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    void Info(string message);

    /// <summary>
    /// 警告レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    void Warn(string message);

    /// <summary>
    /// エラーレベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="exception">関連する例外（省略可）</param>
    void Error(string message, Exception? exception = null);

    /// <summary>
    /// 重大レベルのメッセージを記録します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="exception">関連する例外（省略可）</param>
    void Critical(string message, Exception? exception = null);
}

namespace Ateliers.Logging;

/// <summary>
/// 複数のロガーにログを転送するロガーを表します。
/// </summary>
public class CompositeLogger : LoggerBase
{
    private readonly IReadOnlyList<ILogger> _loggers;

    /// <summary>
    /// CompositeLogger の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="loggers">転送先のロガーのコレクション</param>
    /// <param name="category">ロガーのカテゴリ</param>
    /// <exception cref="ArgumentNullException"><paramref name="loggers"/> が null の場合</exception>
    public CompositeLogger(IEnumerable<ILogger> loggers, string? category = null)
        : base(category)
    {
        _loggers = loggers?.ToList()
            ?? throw new ArgumentNullException(nameof(loggers));
    }

    /// <inheritdoc/>
    public override void Log(LogEntry entry)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.Log(entry);
            }
            catch
            {
                // ログ失敗でプロセスを止めない
            }
        }
    }
}

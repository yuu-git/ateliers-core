namespace Ateliers.Logging;

/// <summary>
/// ロガーの抽象基底クラス
/// </summary>
public abstract class LoggerBase : ILogger
{
    /// <summary>
    /// ロガーのカテゴリを取得します。
    /// </summary>
    protected string? Category { get; }

    /// <summary>
    /// LoggerBase の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="category">ロガーのカテゴリ</param>
    protected LoggerBase(string? category = null)
    {
        Category = category;
    }

    /// <summary>
    /// ログエントリを記録します。
    /// </summary>
    /// <param name="entry">ログエントリ</param>
    public abstract void Log(LogEntry entry);

    /// <inheritdoc/>
    public virtual void Trace(string message) 
        => Log(LogEntryFactory.Create(LogLevel.Trace, message, null, Category));

    /// <inheritdoc/>
    public virtual void Debug(string message) 
        => Log(LogEntryFactory.Create(LogLevel.Debug, message, null, Category));

    /// <inheritdoc/>
    public virtual void Info(string message) 
        => Log(LogEntryFactory.Create(LogLevel.Information, message, null, Category));

    /// <inheritdoc/>
    public virtual void Warn(string message) 
        => Log(LogEntryFactory.Create(LogLevel.Warning, message, null, Category));

    /// <inheritdoc/>
    public virtual void Error(string message, Exception? exception = null) 
        => Log(LogEntryFactory.Create(LogLevel.Error, message, exception, Category));

    /// <inheritdoc/>
    public virtual void Critical(string message, Exception? exception = null) 
        => Log(LogEntryFactory.Create(LogLevel.Critical, message, exception, Category));
}

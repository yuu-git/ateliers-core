namespace Ateliers.Logging;

/// <summary>
/// コンソールにログを出力するロガーを表します。
/// </summary>
public class ConsoleLogger : LoggerBase
{
    private readonly LoggerOptions _options;

    /// <summary>
    /// コンソールロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    public ConsoleLogger(LoggerOptions options)
        : base(options.Category)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public override void Log(LogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        var categoryPrefix = !string.IsNullOrEmpty(entry.Category) ? $"[{entry.Category}] " : "";
        var prefix = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {categoryPrefix}";
        Console.WriteLine($"{prefix}{entry.LogText}");

        if (entry.Exception != null)
        {
            Console.WriteLine(entry.Exception);
        }
    }
}

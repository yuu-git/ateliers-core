namespace Ateliers.Logging;

/// <summary>
/// メモリ内にログを管理するロガー
/// </summary>
public class InMemoryLogger : LoggerBase
{
    private readonly LoggerOptions _options;
    private readonly List<LogEntry> _entries = new();

    /// <summary>
    /// ログ エントリの読み取り専用リストを取得します。
    /// </summary>
    public IReadOnlyList<LogEntry> Entries => _entries;

    /// <summary>
    /// メモリ内ロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    public InMemoryLogger(LoggerOptions options)
        : base(options.Category)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public override void Log(LogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        _entries.Add(entry);
    }

    /// <summary>
    /// すべてのログをクリアします。
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
    }
}

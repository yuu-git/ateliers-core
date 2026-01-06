namespace Ateliers.Logging;

/// <summary>
/// ロガーのオプションを表します。
/// </summary>
public class LoggerOptions
{
    /// <summary>
    /// 記録する最小のログ レベルを取得または設定します。
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// ログファイルを保存するディレクトリのパスを取得または設定します。
    /// </summary>
    public string? LogDirectory { get; set; }

    /// <summary>
    /// コンソールへのログ出力を有効にするかどうかを取得または設定します。
    /// </summary>
    public bool EnableConsole { get; set; }

    /// <summary>
    /// ログのカテゴリを取得または設定します（例: "MCP", "VoiceEngine", "App"）。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    public LoggerOptions()
    {
    }

    /// <summary>
    /// ロガーのオプションの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="minimumLevel">記録する最小のログレベル</param>
    /// <param name="logDirectory">ログファイルを保存するディレクトリのパス</param>
    /// <param name="enableConsole">コンソールへのログ出力を有効にするかどうか</param>
    /// <param name="category">ログのカテゴリ</param>
    public LoggerOptions(LogLevel minimumLevel, string? logDirectory, bool enableConsole, string? category = null)
    {
        MinimumLevel = minimumLevel;
        LogDirectory = logDirectory;
        EnableConsole = enableConsole;
        Category = category;
    }
}

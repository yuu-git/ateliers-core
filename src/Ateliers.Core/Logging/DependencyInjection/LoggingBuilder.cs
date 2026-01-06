namespace Ateliers.Logging.DependencyInjection;

/// <summary>
/// ロギング ビルダーを表します。
/// </summary>
public class LoggingBuilder
{
    internal IList<ILogger> Loggers { get; } = new List<ILogger>();
    internal LoggerOptions Options { get; } = new();

    /// <summary>
    /// 最小ログレベルを設定します。
    /// </summary>
    /// <param name="level">ログレベル</param>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder SetMinimumLevel(LogLevel level)
    {
        Options.MinimumLevel = level;
        return this;
    }

    /// <summary>
    /// ログのカテゴリを設定します。
    /// </summary>
    /// <param name="category">カテゴリ（例: "App", "VoiceEngine"）</param>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder SetCategory(string category)
    {
        Options.Category = category;
        return this;
    }

    /// <summary>
    /// コンソール ロガーを追加します。
    /// </summary>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder AddConsole()
    {
        Loggers.Add(new ConsoleLogger(Options));
        return this;
    }

    /// <summary>
    /// ファイル ロガーを追加します。
    /// </summary>
    /// <param name="logDirectory">ログ ディレクトリのパス。指定しない場合、デフォルトのログ ディレクトリを使用します</param>
    /// <param name="filePrefix">ログファイル名のプレフィックス（デフォルト: "log"）</param>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder AddFile(string? logDirectory = null, string filePrefix = "log")
    {
        var options = new LoggerOptions
        {
            MinimumLevel = Options.MinimumLevel,
            LogDirectory = logDirectory,
            EnableConsole = Options.EnableConsole,
            Category = Options.Category
        };
        Loggers.Add(new FileLogger(options, filePrefix));
        return this;
    }

    /// <summary>
    /// インメモリ ロガーを追加します。
    /// </summary>
    /// <param name="logger">追加されたインメモリ ロガー</param>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder AddInMemory(out InMemoryLogger logger)
    {
        logger = new InMemoryLogger(Options);
        Loggers.Add(logger);
        return this;
    }

    /// <summary>
    /// カスタム ロガーを追加します。
    /// </summary>
    /// <param name="logger">追加するロガー</param>
    /// <returns>このビルダーのインスタンス</returns>
    public LoggingBuilder AddLogger(ILogger logger)
    {
        Loggers.Add(logger);
        return this;
    }
}

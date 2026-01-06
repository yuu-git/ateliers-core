using System.Text;

namespace Ateliers.Logging;

/// <summary>
/// ファイルにログを管理するロガー
/// </summary>
public class FileLogger : LoggerBase
{
    private readonly string _logDir;
    private readonly LoggerOptions _options;
    private readonly string _filePrefix;

    /// <summary>
    /// ファイルロガー の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">ロガーのオプション</param>
    /// <param name="filePrefix">ログファイル名のプレフィックス（デフォルト: "log"）</param>
    public FileLogger(LoggerOptions options, string filePrefix = "log")
        : base(options.Category)
    {
        _options = options;
        _filePrefix = filePrefix;
        _logDir = options.LogDirectory ?? Path.Combine(AppContext.BaseDirectory, "logs", "app");

        Directory.CreateDirectory(_logDir);
    }

    /// <inheritdoc/>
    public override void Log(LogEntry entry)
    {
        if (entry.Level < _options.MinimumLevel)
            return;

        var filePath = Path.Combine(
            _logDir,
            $"{_filePrefix}-{DateTime.Now:yyyy-MM-dd}.log"
        );

        var sb = new StringBuilder();
        sb.Append($"[{entry.Timestamp:O}] [{entry.Level}] ");
        
        if (!string.IsNullOrEmpty(entry.Category))
        {
            sb.Append($"[{entry.Category}] ");
        }

        if (!string.IsNullOrEmpty(entry.CorrelationId))
        {
            sb.Append($"[CID:{entry.CorrelationId}] ");
        }

        sb.Append(entry.LogText);

        if (entry.Exception != null)
        {
            sb.AppendLine();
            sb.Append(entry.Exception);
        }

        File.AppendAllText(filePath, sb.ToString() + Environment.NewLine);
    }
}

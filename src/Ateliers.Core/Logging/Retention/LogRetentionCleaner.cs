namespace Ateliers.Logging.Retention;

/// <summary>
/// ログの保持クリーナーを表します。
/// </summary>
public class LogRetentionCleaner
{
    private readonly string _baseLogDirectory;
    private readonly LogRetentionPolicy _policy;

    /// <summary>
    /// LogRetentionCleaner の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="baseLogDirectory">ログの基底ディレクトリ</param>
    /// <param name="policy">ログ保持ポリシー</param>
    public LogRetentionCleaner(
        string baseLogDirectory,
        LogRetentionPolicy policy)
    {
        _baseLogDirectory = baseLogDirectory;
        _policy = policy;
    }

    /// <summary>
    /// ログのクリーンアップを実行します。
    /// </summary>
    public void Clean()
    {
        if (!Directory.Exists(_baseLogDirectory))
            return;

        CleanDirectory("trace", _policy.TraceRetention);
        CleanDirectory("debug", _policy.DebugRetention);
        CleanDirectory("app", _policy.InformationRetention);
        CleanDirectory("warning", _policy.WarningRetention);
        CleanDirectory("error", _policy.ErrorRetention);
        CleanDirectory("critical", _policy.CriticalRetention);
    }

    /// <summary>
    /// 指定されたサブディレクトリをクリーンアップします。
    /// </summary>
    /// <param name="subDir">サブディレクトリ名</param>
    /// <param name="retention">保持期間</param>
    protected void CleanDirectory(string subDir, TimeSpan retention)
    {
        var dir = Path.Combine(_baseLogDirectory, subDir);
        if (!Directory.Exists(dir))
            return;

        var now = DateTimeOffset.UtcNow;

        foreach (var file in Directory.EnumerateFiles(dir))
        {
            var lastWriteUtc = File.GetLastWriteTimeUtc(file);
            var lastWrite = new DateTimeOffset(lastWriteUtc, TimeSpan.Zero);

            if (now - lastWrite > retention)
            {
                TryDelete(file);
            }
        }
    }

    private static void TryDelete(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch
        {
            // 削除失敗は無視（ログで死なない）
        }
    }
}

namespace Ateliers.Logging;

/// <summary>
/// ログレベル
/// </summary>
public enum LogLevel
{
    /// <summary> 不明なログレベル </summary>
    Unknown,
    /// <summary> トレースログ </summary>
    Trace,
    /// <summary> デバッグログ </summary>
    Debug,
    /// <summary> 情報ログ </summary>
    Information,
    /// <summary> 警告ログ </summary>
    Warning,
    /// <summary> エラーログ </summary>
    Error,
    /// <summary> 重大ログ </summary>
    Critical
}

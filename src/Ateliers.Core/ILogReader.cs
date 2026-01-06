namespace Ateliers.Logging;

/// <summary>
/// ログリーダーを表します。
/// </summary>
public interface ILogReader
{
    /// <summary>
    /// 指定された相関IDに基づいてログセッションを読み取ります。
    /// </summary>
    /// <param name="correlationId">読み取りする相関ID</param>
    /// <returns>指定された相関IDに基づくログセッション</returns>
    LogSession ReadByCorrelationId(string correlationId);

    /// <summary>
    /// 最後のログセッションを読み取ります。
    /// </summary>
    /// <returns>最後のログセッション</returns>
    LogSession ReadLastSession();

    /// <summary>
    /// 指定されたカテゴリに基づいてログセッションを読み取ります。
    /// </summary>
    /// <param name="category">読み取りするカテゴリ</param>
    /// <returns>指定されたカテゴリに基づくログセッション</returns>
    LogSession ReadByCategory(string category);

    /// <summary>
    /// 指定された相関IDとカテゴリに基づいてログセッションを読み取ります。
    /// </summary>
    /// <param name="correlationId">読み取りする相関ID</param>
    /// <param name="category">読み取りするカテゴリ</param>
    /// <returns>指定された相関IDとカテゴリに基づくログセッション</returns>
    LogSession ReadByCorrelationIdAndCategory(string correlationId, string category);
}

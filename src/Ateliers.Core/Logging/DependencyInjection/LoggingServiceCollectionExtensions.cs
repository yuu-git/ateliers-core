using Microsoft.Extensions.DependencyInjection;

namespace Ateliers.Logging.DependencyInjection;

/// <summary>
/// IServiceCollection 用のロギング拡張メソッドを提供します。
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    /// ロギングをサービス コレクションに追加します。
    /// </summary>
    /// <param name="services">サービス コレクション</param>
    /// <param name="configure">ロギング ビルダーの構成アクション</param>
    /// <returns>更新されたサービス コレクション</returns>
    public static IServiceCollection AddAteliersLogging(
        this IServiceCollection services,
        Action<LoggingBuilder>? configure = null)
    {
        var builder = new LoggingBuilder();

        configure?.Invoke(builder);

        if (builder.Loggers.Count == 0)
        {
            builder.AddConsole();
        }

        ILogger logger = builder.Loggers.Count == 1
            ? builder.Loggers[0]
            : new CompositeLogger(builder.Loggers, builder.Options.Category);

        services.AddSingleton(logger);

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Ateliers.DependencyInjection;

/// <summary>
/// IServiceCollection 用の実行コンテキスト拡張メソッドを提供します。
/// </summary>
public static class ExecutionContextServiceCollectionExtensions
{
    /// <summary>
    /// 実行コンテキストをサービス コレクションに追加します。
    /// </summary>
    /// <param name="services">サービス コレクション</param>
    /// <returns>更新されたサービス コレクション</returns>
    public static IServiceCollection AddAteliersExecutionContext(
        this IServiceCollection services)
    {
        services.AddScoped<IExecutionContext>(provider =>
        {
            var current = Context.ExecutionContext.Current;
            if (current != null)
            {
                return current;
            }

            // コンテキストがない場合は新規作成
            var correlationId = Guid.NewGuid().ToString();
            return new Context.ExecutionContext(correlationId, null);
        });

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus;
using Renligou.Core.Shared.Bus;

/// <summary>
/// 依赖注入扩展。
/// </summary>
namespace Renligou.Core.Application.DependencyInjection;

/// <summary>
/// 为 IServiceCollection 提供扩展方法以注册命令查询总线。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Renligou 命令查询总线核心组件。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddRenligouCommandQueryBus(this IServiceCollection services)
    {
        // 注册为 Scoped 以支持从请求作用域解析 Handler
        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();

        return services;
    }
}

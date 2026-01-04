using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Shared.Pipeline;

/// <summary>
/// 管道执行器内部实现。
/// </summary>
namespace Renligou.Core.Application.Bus.Internal;

/// <summary>
/// 负责构建并执行请求处理管道。
/// </summary>
internal static class PipelineInvoker
{
    /// <summary>
    /// 调用包含管道行为的链。
    /// </summary>
    /// <typeparam name="TRequest">请求类型。</typeparam>
    /// <typeparam name="TResponse">响应类型。</typeparam>
    /// <param name="serviceProvider">服务提供者。</param>
    /// <param name="request">请求实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <param name="handlerAction">最终的处理委托。</param>
    /// <returns>返回结果。</returns>
    public static Task<TResponse> InvokeAsync<TRequest, TResponse>(
        IServiceProvider serviceProvider,
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> handlerAction)
    {
        // 尝试解析所有行为。
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();
        
        // 转换为 IReadOnlyList 以利用索引访问，减少迭代开销
        if (behaviors is not IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> list)
        {
            var tempList = new List<IPipelineBehavior<TRequest, TResponse>>();
            foreach (var behavior in behaviors)
            {
                tempList.Add(behavior);
            }
            list = tempList;
        }

        if (list.Count == 0)
        {
            return handlerAction();
        }

        // 使用索引反向构建委托链
        Func<Task<TResponse>> next = handlerAction;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var behavior = list[i];
            var currentNext = next;
            next = () => behavior.HandleAsync(request, cancellationToken, currentNext);
        }

        return next();
    }
}

/// <summary>
/// 管道行为接口定义。
/// </summary>
namespace Renligou.Core.Shared.Pipeline;

/// <summary>
/// 定义请求处理管道中的一个行为。
/// </summary>
/// <typeparam name="TRequest">请求类型。</typeparam>
/// <typeparam name="TResponse">响应类型。</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
{
    /// <summary>
    /// 处理请求。
    /// </summary>
    /// <param name="request">请求实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <param name="next">指向下一个行为或最终处理器的委托。</param>
    /// <returns>响应结果。</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next);
}

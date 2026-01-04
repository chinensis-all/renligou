/// <summary>
/// 查询处理器接口定义。
/// </summary>
namespace Renligou.Core.Shared.Querying;

/// <summary>
/// 定义查询处理器。
/// </summary>
/// <typeparam name="TQuery">查询类型。</typeparam>
/// <typeparam name="TResult">查询结果类型。</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// 处理查询。
    /// </summary>
    /// <param name="query">查询实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>查询结果。</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}

using Renligou.Core.Shared.Querying;

/// <summary>
/// 查询总线接口。
/// </summary>
namespace Renligou.Core.Shared.Bus;

/// <summary>
/// 提供查询派发入口。
/// </summary>
public interface IQueryBus
{
    /// <summary>
    /// 派发查询。
    /// </summary>
    /// <typeparam name="TQuery">查询类型。</typeparam>
    /// <typeparam name="TResult">查询结果类型。</typeparam>
    /// <param name="query">查询实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>查询结果。</returns>
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}

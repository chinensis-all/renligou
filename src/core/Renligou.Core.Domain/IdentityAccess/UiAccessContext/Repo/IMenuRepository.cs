using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;

/// <summary>
/// 菜单领域仓储接口
/// </summary>
public interface IMenuRepository : IRepository, DomainRepository<Menu>
{
    /// <summary>
    /// 检查菜单名称和标识联合冲突
    /// </summary>
    /// <param name="menuName">菜单名称</param>
    /// <param name="menuTag">菜单标识</param>
    /// <param name="excludeId">排除ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否冲突</returns>
    Task<bool> IsNameTagConflictAsync(string menuName, string menuTag, long? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查菜单是否存在
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
}

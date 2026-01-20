using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Repo;

/// <summary>
/// 角色领域仓储接口
/// </summary>
public interface IRoleRepository : IRepository, DomainRepository<Role>
{
    /// <summary>
    /// 检查角色名称是否冲突 (唯一性)
    /// </summary>
    /// <param name="id">当前角色ID</param>
    /// <param name="roleName">待检查的角色名称</param>
    /// <returns>是否存在冲突</returns>
    Task<bool> IsRoleNameConflictAsync(long id, string roleName);

    /// <summary>
    /// 检查角色显示名称是否冲突 (唯一性)
    /// </summary>
    /// <param name="id">当前角色ID</param>
    /// <param name="displayName">待检查的角色显示名称</param>
    /// <returns>是否存在冲突</returns>
    Task<bool> IsDisplayNameConflictAsync(long id, string displayName);
}

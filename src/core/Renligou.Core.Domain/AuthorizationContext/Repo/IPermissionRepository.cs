using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Repo
{
    /// <summary>
    /// 权限领域仓储接口
    /// </summary>
    public interface IPermissionRepository : IRepository, DomainRepository<Permission>
    {
        /// <summary>
        /// 检查权限标识是否冲突
        /// </summary>
        Task<bool> IsPermissionNameConflictAsync(long id, string permissionName);

        /// <summary>
        /// 检查权限名称是否冲突
        /// </summary>
        Task<bool> IsDisplayNameConflictAsync(long id, string displayName);

        /// <summary>
        /// 检查指定权限组及其子组下是否存在权限
        /// </summary>
        Task<bool> HasPermissionsAsync(long groupId, CancellationToken cancellationToken);
    }
}

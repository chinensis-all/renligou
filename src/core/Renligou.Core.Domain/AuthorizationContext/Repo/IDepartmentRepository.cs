using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Repo;

/// <summary>
/// 部门领域仓储接口
/// </summary>
public interface IDepartmentRepository : IRepository, DomainRepository<Department>
{
    /// <summary>
    /// 检查部门名称在同一公司同一父部门下是否存在冲突
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <param name="parentId">父部门ID</param>
    /// <param name="deptName">部门名称</param>
    /// <param name="excludeId">排除的ID（用于更新时）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在冲突</returns>
    Task<bool> IsDeptNameConflictAsync(long companyId, long parentId, string deptName, long? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查部门是否存在
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
}

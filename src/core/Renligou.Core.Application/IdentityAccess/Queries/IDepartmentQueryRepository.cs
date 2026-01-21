using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 部门查询仓储接口
/// </summary>
public interface IDepartmentQueryRepository : IRepository
{
    /// <summary>
    /// 查询部门详情
    /// </summary>
    Task<DepartmentDetailDto?> QueryDetailAsync(long departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索部门列表
    /// </summary>
    Task<List<DepartmentListDto>> SearchAsync(long? companyId, string? deptName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取部门树
    /// </summary>
    Task<List<DepartmentTreeNodeDto>> GetDepartmentTreeAsync(DepartmentTreeCriteria criteria, CancellationToken cancellationToken = default);
}

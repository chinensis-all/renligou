using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取部门树处理器
/// </summary>
public sealed class GetDepartmentTreeHandler(
    IDepartmentQueryRepository _departmentQueryRepository
) : IQueryHandler<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>
{
    public async Task<List<DepartmentTreeNodeDto>> HandleAsync(GetDepartmentTreeQuery query, CancellationToken cancellationToken)
    {
        var criteria = new DepartmentTreeCriteria
        {
            ParentId = query.ParentId,
            Name = query.Name,
            CompanyId = query.CompanyId
        };

        return await _departmentQueryRepository.GetDepartmentTreeAsync(criteria, cancellationToken);
    }
}

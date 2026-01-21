using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取部门列表处理器
/// </summary>
public sealed class GetDepartmentListHandler(
    IDepartmentQueryRepository _departmentQueryRepository
) : IQueryHandler<GetDepartmentListQuery, Result<List<DepartmentListDto>>>
{
    public async Task<Result<List<DepartmentListDto>>> HandleAsync(GetDepartmentListQuery query, CancellationToken cancellationToken)
    {
        var list = await _departmentQueryRepository.SearchAsync(query.CompanyId, query.DeptName, cancellationToken);
        return Result<List<DepartmentListDto>>.Ok(list);
    }
}

using System.Collections.Generic;
using Renligou.Core.Shared.Querying;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取部门列表查询
/// </summary>
public sealed record GetDepartmentListQuery : IQuery<Result<List<DepartmentListDto>>>
{
    public long? CompanyId { get; init; }
    public string? DeptName { get; init; }
}

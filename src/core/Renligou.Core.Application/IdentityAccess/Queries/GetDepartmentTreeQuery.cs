using System.Collections.Generic;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取部门树查询
/// </summary>
public sealed record GetDepartmentTreeQuery : IQuery<List<DepartmentTreeNodeDto>>
{
    public long ParentId { get; init; }
    public string? Name { get; init; }
    public long? CompanyId { get; init; }
}

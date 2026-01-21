namespace Renligou.Core.Application.IdentityAccess.Criterias;

/// <summary>
/// 部门树查询条件
/// </summary>
public sealed record DepartmentTreeCriteria
{
    public long ParentId { get; init; }
    public string? Name { get; init; }
    public long? CompanyId { get; init; }
}

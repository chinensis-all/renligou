using System.Collections.Generic;
using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 部门树节点 DTO
/// </summary>
public sealed record DepartmentTreeNodeDto
{
    [Description("节点ID")]
    public long Id { get; init; }

    [Description("父节点ID")]
    public long ParentId { get; init; }

    [Description("部门名称")]
    public string Name { get; init; } = string.Empty;

    [Description("子节点列表")]
    public List<DepartmentTreeNodeDto> Children { get; init; } = new();
}

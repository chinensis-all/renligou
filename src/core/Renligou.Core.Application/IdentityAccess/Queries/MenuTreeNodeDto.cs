using System.Collections.Generic;
using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 菜单树节点 DTO
/// </summary>
public record MenuTreeNodeDto
{
    [Description("菜单ID")]
    public long Id { get; init; }

    [Description("父级菜单ID")]
    public long ParentId { get; init; }

    [Description("菜单名称")]
    public string Name { get; init; } = string.Empty;

    [Description("标识")]
    public string Tag { get; init; } = string.Empty;

    [Description("图标")]
    public string Icon { get; init; } = string.Empty;

    [Description("路径")]
    public string Path { get; init; } = string.Empty;

    [Description("是否隐藏")]
    public bool IsHidden { get; init; }

    [Description("子节点")]
    public List<MenuTreeNodeDto> Children { get; init; } = new();
}

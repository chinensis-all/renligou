using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 菜单详情 DTO
/// </summary>
public record MenuDetailDto
{
    [Description("菜单ID")]
    public long Id { get; init; }

    [Description("父级菜单ID")]
    public long ParentId { get; init; }

    [Description("菜单名称")]
    public string MenuName { get; init; } = string.Empty;

    [Description("菜单标识")]
    public string MenuTag { get; init; } = string.Empty;

    [Description("菜单路径")]
    public string Path { get; init; } = string.Empty;

    [Description("组件路径")]
    public string Component { get; init; } = string.Empty;

    [Description("图标")]
    public string Icon { get; init; } = string.Empty;

    [Description("排序号")]
    public int Sorter { get; init; }

    [Description("是否隐藏")]
    public bool IsHidden { get; init; }

    [Description("按钮权限")]
    public string PermitButtons { get; init; } = string.Empty;

    [Description("创建时间")]
    public DateTimeOffset CreatedAt { get; init; }

    [Description("更新时间")]
    public DateTimeOffset UpdatedAt { get; init; }
}

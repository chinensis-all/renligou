using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests;

/// <summary>
/// 创建菜单请求
/// </summary>
public record CreateMenuRequest
{
    /// <summary>
    /// 父级菜单ID (0表示顶层)
    /// </summary>
    public long ParentId { get; init; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    public string MenuName { get; init; } = string.Empty;

    /// <summary>
    /// 菜单标识 (用于区分同名菜单)
    /// </summary>
    public string MenuTag { get; init; } = string.Empty;

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>
    /// 前端组件路径
    /// </summary>
    public string Component { get; init; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; init; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sorter { get; init; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHidden { get; init; }

    /// <summary>
    /// 按钮权限 (多个逗号分隔)
    /// </summary>
    public string PermitButtons { get; init; } = string.Empty;
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests;

/// <summary>
/// 创建角色请求
/// </summary>
public record CreateRoleRequest
{
    [Description("角色名称")]
    [Required(ErrorMessage = "角色名称不能为空")]
    [MaxLength(100, ErrorMessage = "角色名称不能超过100个字符")]
    public string RoleName { get; init; } = string.Empty;

    [Description("显示名称 (权限字符串)")]
    [Required(ErrorMessage = "显示名称不能为空")]
    [MaxLength(100, ErrorMessage = "显示名称不能超过100个字符")]
    public string DisplayName { get; init; } = string.Empty;
}

/// <summary>
/// 修改角色基础信息请求
/// </summary>
public record ModifyRoleBasicRequest
{
    [Description("角色名称")]
    [Required(ErrorMessage = "角色名称不能为空")]
    [MaxLength(100, ErrorMessage = "角色名称不能超过100个字符")]
    public string RoleName { get; init; } = string.Empty;

    [Description("显示名称 (权限字符串)")]
    [Required(ErrorMessage = "显示名称不能为空")]
    [MaxLength(100, ErrorMessage = "显示名称不能超过100个字符")]
    public string DisplayName { get; init; } = string.Empty;
}

/// <summary>
/// 角色分页查询请求
/// </summary>
public record GetRolePageRequest
{
    [Description("关键字 (角色名或显示名)")]
    public string? Keyword { get; init; }

    [Description("页码")]
    public int Page { get; init; } = 1;

    [Description("每页行数")]
    public int PageSize { get; init; } = 20;
}

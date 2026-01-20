using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 修改角色基础信息请求
    /// </summary>
    public sealed record ModifyRoleBasicRequest
    {
        [Description("角色名称")]
        [Required(ErrorMessage = "角色名称不能为空")]
        [MaxLength(100, ErrorMessage = "角色名称不能超过100个字符")]
        public string RoleName { get; init; } = string.Empty;

        [Description("显示名称")]
        [Required(ErrorMessage = "显示名称不能为空")]
        [MaxLength(100, ErrorMessage = "显示名称不能超过100个字符")]
        public string DisplayName { get; init; } = string.Empty;

        [Description("角色描述")]
        [MaxLength(255, ErrorMessage = "角色描述不能超过255个字符")]
        public string Description { get; init; } = string.Empty;
    }
}

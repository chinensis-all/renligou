using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 修改权限请求
    /// </summary>
    public sealed record ModifyPermissionRequest
    {
        [Description("权限组ID")]
        [Required(ErrorMessage = "权限组ID不能为空")]
        public long GroupId { get; init; }

        [Description("权限标识")]
        [Required(ErrorMessage = "权限标识不能为空")]
        [MaxLength(100, ErrorMessage = "权限标识不能超过100个字符")]
        public string PermissionName { get; init; } = string.Empty;

        [Description("权限名称")]
        [Required(ErrorMessage = "权限名称不能为空")]
        [MaxLength(100, ErrorMessage = "权限名称不能超过100个字符")]
        public string DisplayName { get; init; } = string.Empty;

        [Description("权限描述")]
        [MaxLength(500, ErrorMessage = "权限描述不能超过500个字符")]
        public string Description { get; init; } = string.Empty;
    }
}

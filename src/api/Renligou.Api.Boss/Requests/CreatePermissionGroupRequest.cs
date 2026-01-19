using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    public class CreatePermissionGroupRequest
    {
        [Description("权限组名称")]
        [Required(ErrorMessage = "权限组名称不能为空")]
        [MaxLength(64, ErrorMessage = "权限组名称长度不能超过 100")]
        public string GroupName { get; set; } = string.Empty;

        [Description("权限组显示名称")]
        [Required(ErrorMessage = "权限组显示名称不能为空")]
        [MaxLength(100, ErrorMessage = "权限组显示名称长度不能超过 100")]
        public string DisplayName { get; set; } = string.Empty;

        [Description("权限组描述")]
        [MaxLength(255, ErrorMessage = "权限组描述长度不能超过 255")]
        public string Description { get; set; } = string.Empty;

        [Description("父权限组ID")]
        public long ParentId { get; set; }

        [Description("排序")]
        public int Sorter { get; set; }
    }
}

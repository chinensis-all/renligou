using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    public sealed record PermissionGroupListDto
    {
        [Description("权限组ID")]
        public string Id { get; set; } = string.Empty;

        [Description("权限组名称")]
        public string GroupName { get; set; } = string.Empty;

        [Description("权限组显示名称")]
        public string DisplayName { get; set; } = string.Empty;

        [Description("父权限组ID")]
        public string ParentId { get; set; } = string.Empty;

        [Description("排序")]
        public int Sorter { get; set; }
    }
}

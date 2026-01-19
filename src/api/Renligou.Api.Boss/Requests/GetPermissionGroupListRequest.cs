using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    public class GetPermissionGroupListRequest
    {
        [Description("权限组名称")]
        public string? GroupName { get; set; }

        [Description("权限组显示名称")]
        public string? DisplayName { get; set; }
    }
}

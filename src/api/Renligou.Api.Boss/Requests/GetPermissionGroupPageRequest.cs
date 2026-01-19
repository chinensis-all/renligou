using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    public class GetPermissionGroupPageRequest
    {
        [Description("权限组名称")]
        public string? GroupName { get; set; }

        [Description("权限组显示名称")]
        public string? DisplayName { get; set; }

        [Description("页码")]
        public int Page { get; set; } = 1;

        [Description("每页大小")]
        public int PageSize { get; set; } = 10;
    }
}

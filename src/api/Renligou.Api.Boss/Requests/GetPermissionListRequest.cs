using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 获取权限列表请求
    /// </summary>
    public sealed record GetPermissionListRequest
    {
        [Description("权限组ID")]
        public long? GroupId { get; init; }

        [Description("权限标识")]
        public string? PermissionName { get; init; }

        [Description("权限名称")]
        public string? DisplayName { get; init; }
    }
}

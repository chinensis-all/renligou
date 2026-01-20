using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 获取权限分页请求
    /// </summary>
    public sealed record GetPermissionPageRequest
    {
        [Description("权限组ID")]
        public long? GroupId { get; init; }

        [Description("权限标识")]
        public string? PermissionName { get; init; }

        [Description("权限名称")]
        public string? DisplayName { get; init; }

        [Description("页码")]
        public int Page { get; init; } = 1;

        [Description("页大小")]
        public int PageSize { get; init; } = 10;
    }
}

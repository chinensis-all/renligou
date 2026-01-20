using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 角色分页查询请求
    /// </summary>
    public sealed record GetRolePageRequest
    {
        [Description("关键字 (角色名或显示名)")]
        public string? Keyword { get; init; }

        [Description("页码")]
        public int Page { get; init; } = 1;

        [Description("每页行数")]
        public int PageSize { get; init; } = 20;
    }
}

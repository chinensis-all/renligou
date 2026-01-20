using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    /// <summary>
    /// 获取角色列表请求
    /// </summary>
    public class GetRoleListRequest
    {
        [Description("角色标识")]
        public string? RoleName { get; init; }

        [Description("角色显示名称")]
        public string? DisplayName { get; init; }
    }
}

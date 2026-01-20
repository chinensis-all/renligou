using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 角色详情 DTO
    /// </summary>
    public record RoleDetailDto
    {
        [Description("角色ID")]
        public long Id { get; init; }

        [Description("角色名称")]
        public string RoleName { get; init; } = string.Empty;

        [Description("显示名称")]
        public string DisplayName { get; init; } = string.Empty;

        [Description("描述")]
        public string Description { get; init; } = string.Empty;

        [Description("创建时间")]
        public DateTimeOffset CreatedAt { get; init; }

        [Description("最后更新时间")]
        public DateTimeOffset UpdatedAt { get; init; }
    }
}

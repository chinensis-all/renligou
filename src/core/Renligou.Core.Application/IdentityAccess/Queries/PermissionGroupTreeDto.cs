using System.Collections.Generic;
using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 权限组树节点DTO
    /// </summary>
    public sealed record PermissionGroupTreeDto
    {
        [Description("权限组ID")]
        public long Id { get; init; }

        [Description("父权限组ID")]
        public long ParentId { get; init; }

        [Description("权限组名称")]
        public string GroupName { get; init; } = string.Empty;

        [Description("显示名称")]
        public string DisplayName { get; init; } = string.Empty;

        [Description("描述")]
        public string? Description { get; init; }

        [Description("排序号")]
        public int Sorter { get; init; }

        [Description("子权限组")]
        public List<PermissionGroupTreeDto> Children { get; init; } = new();
    }
}

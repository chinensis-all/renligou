using System.Collections.Generic;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 权限组树节点DTO
    /// </summary>
    public sealed record PermissionGroupTreeDto
    {
        /// <summary>
        /// 权限组ID
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// 父权限组ID
        /// </summary>
        public long ParentId { get; init; }

        /// <summary>
        /// 权限组名称
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sorter { get; init; }

        /// <summary>
        /// 子权限组
        /// </summary>
        public List<PermissionGroupTreeDto> Children { get; init; } = new();
    }
}

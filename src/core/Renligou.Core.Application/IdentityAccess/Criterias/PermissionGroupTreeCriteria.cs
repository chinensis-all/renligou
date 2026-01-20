namespace Renligou.Core.Application.IdentityAccess.Criterias
{
    /// <summary>
    /// 权限组树查询条件
    /// </summary>
    public sealed record PermissionGroupTreeCriteria
    {
        /// <summary>
        /// 父ID
        /// </summary>
        public long ParentId { get; init; }

        /// <summary>
        /// 权限组名称/显示名称（模糊匹配）
        /// </summary>
        public string? Name { get; init; }
    }
}

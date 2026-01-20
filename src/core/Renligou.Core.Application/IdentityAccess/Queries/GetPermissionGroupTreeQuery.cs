using System.Collections.Generic;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 获取权限组树形结构查询
    /// </summary>
    public sealed record GetPermissionGroupTreeQuery : IQuery<Result<List<PermissionGroupTreeDto>>>
    {
        /// <summary>
        /// 父权限组ID
        /// </summary>
        public long ParentId { get; init; }

        /// <summary>
        /// 权限组名称（模糊匹配）
        /// </summary>
        public string? Name { get; init; }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Shared.Common;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    public interface IPermissionGroupQueryRepository : IRepository
    {
        Task<PermissionGroupDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default);

        Task<List<PermissionGroupListDto>> SearchAsync(PermissionGroupSearchCriteria criteria, CancellationToken cancellationToken = default);

        Task<Pagination<PermissionGroupDetailDto>> PaginateAsync(PermissionGroupSearchCriteria searchCriteria, PermissionGroupPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取权限组树
        /// </summary>
        /// <param name="criteria">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>树形结构列表</returns>
        Task<List<PermissionGroupTreeDto>> GetPermissionGroupTreeAsync(PermissionGroupTreeCriteria criteria, CancellationToken cancellationToken = default);
    }
}

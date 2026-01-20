using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 权限查询仓储接口
    /// </summary>
    public interface IPermissionQueryRepository : IRepository
    {
        /// <summary>
        /// 查询权限详情
        /// </summary>
        Task<PermissionDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询权限数量
        /// </summary>
        Task<long> CountAsync(PermissionSearchCriteria criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// 搜索权限
        /// </summary>
        Task<List<PermissionListDto>> SearchAsync(PermissionSearchCriteria criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页搜索权限
        /// </summary>
        Task<Pagination<PermissionDetailDto>> PaginateAsync(PermissionSearchCriteria searchCriteria, PermissionPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default);
    }
}

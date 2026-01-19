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
    }
}

using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class GetPermissionGroupPageHandler(
        IPermissionGroupQueryRepository _permissionGroupQueryRepository
    ) : IQueryHandler<GetPermissionGroupPageQuery, Result<Pagination<PermissionGroupDetailDto>>>
    {
        public async Task<Result<Pagination<PermissionGroupDetailDto>>> HandleAsync(GetPermissionGroupPageQuery query, CancellationToken cancellationToken)
        {
            var searchCriteria = new PermissionGroupSearchCriteria(query.GroupName, query.DisplayName);
            var paginateCriteria = new PermissionGroupPaginateCriteria(query.Page, query.PageSize);

            var pagination = await _permissionGroupQueryRepository.PaginateAsync(searchCriteria, paginateCriteria, cancellationToken);

            return Result<Pagination<PermissionGroupDetailDto>>.Ok(pagination);
        }
    }
}

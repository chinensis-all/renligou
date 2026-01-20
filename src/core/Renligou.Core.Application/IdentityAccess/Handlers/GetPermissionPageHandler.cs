using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public sealed record GetPermissionPageQuery(
        long? GroupId = null,
        string? PermissionName = null,
        string? DisplayName = null,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<Result<Pagination<PermissionDetailDto>>>;

    public class GetPermissionPageHandler(IPermissionQueryRepository _queryRepository)
        : IQueryHandler<GetPermissionPageQuery, Result<Pagination<PermissionDetailDto>>>
    {
        public async Task<Result<Pagination<PermissionDetailDto>>> HandleAsync(GetPermissionPageQuery query, CancellationToken cancellationToken)
        {
            var searchCriteria = new PermissionSearchCriteria
            {
                GroupId = query.GroupId,
                PermissionName = query.PermissionName,
                DisplayName = query.DisplayName
            };

            var paginateCriteria = new PermissionPaginateCriteria
            {
                Page = query.Page,
                PageSize = query.PageSize
            };

            var pagination = await _queryRepository.PaginateAsync(searchCriteria, paginateCriteria, cancellationToken);
            return Result<Pagination<PermissionDetailDto>>.Ok(pagination);
        }
    }
}

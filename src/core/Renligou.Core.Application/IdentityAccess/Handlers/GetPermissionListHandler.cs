using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public sealed record GetPermissionListQuery(
        long? GroupId = null,
        string? PermissionName = null,
        string? DisplayName = null
    ) : IQuery<Result<List<PermissionListDto>>>;

    public class GetPermissionListHandler(IPermissionQueryRepository _queryRepository)
        : IQueryHandler<GetPermissionListQuery, Result<List<PermissionListDto>>>
    {
        public async Task<Result<List<PermissionListDto>>> HandleAsync(GetPermissionListQuery query, CancellationToken cancellationToken)
        {
            var criteria = new PermissionSearchCriteria
            {
                GroupId = query.GroupId,
                PermissionName = query.PermissionName,
                DisplayName = query.DisplayName
            };

            var list = await _queryRepository.SearchAsync(criteria, cancellationToken);
            return Result<List<PermissionListDto>>.Ok(list);
        }
    }
}

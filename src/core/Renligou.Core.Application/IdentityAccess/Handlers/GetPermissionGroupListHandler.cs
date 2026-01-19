using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class GetPermissionGroupListHandler(
        IPermissionGroupQueryRepository _permissionGroupQueryRepository
    ) : IQueryHandler<GetPermissionGroupListQuery, Result<List<PermissionGroupListDto>>>
    {
        public async Task<Result<List<PermissionGroupListDto>>> HandleAsync(GetPermissionGroupListQuery query, CancellationToken cancellationToken)
        {
            var criteria = new PermissionGroupSearchCriteria(query.GroupName, query.DisplayName);
            var list = await _permissionGroupQueryRepository.SearchAsync(criteria, cancellationToken);
            return Result<List<PermissionGroupListDto>>.Ok(list);
        }
    }
}

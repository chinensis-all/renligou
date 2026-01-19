using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class GetPermissionGroupDetailHandler(
        IPermissionGroupQueryRepository _permissionGroupQueryRepository
    ) : IQueryHandler<GetPermissionGroupDetailQuery, Result<PermissionGroupDetailDto?>>
    {
        public async Task<Result<PermissionGroupDetailDto?>> HandleAsync(GetPermissionGroupDetailQuery query, CancellationToken cancellationToken)
        {
            var dto = await _permissionGroupQueryRepository.QueryDetailAsync(query.Id, cancellationToken);
            if (dto == null)
            {
                return Result<PermissionGroupDetailDto?>.Fail("PermissionGroup.NotFound", "未找到权限组");
            }
            return Result<PermissionGroupDetailDto?>.Ok(dto);
        }
    }
}

using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public sealed record GetPermissionDetailQuery(long Id) : IQuery<Result<PermissionDetailDto?>>;

    public class GetPermissionDetailHandler(IPermissionQueryRepository _queryRepository) 
        : IQueryHandler<GetPermissionDetailQuery, Result<PermissionDetailDto?>>
    {
        public async Task<Result<PermissionDetailDto?>> HandleAsync(GetPermissionDetailQuery query, CancellationToken cancellationToken)
        {
            var dto = await _queryRepository.QueryDetailAsync(query.Id, cancellationToken);
            if (dto == null)
            {
                return Result<PermissionDetailDto?>.Fail("Permission.NotFound", "未找到权限");
            }

            return Result<PermissionDetailDto?>.Ok(dto);
        }
    }
}

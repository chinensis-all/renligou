using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    /// <summary>
    /// 获取权限组树处理器
    /// </summary>
    public sealed class GetPermissionGroupTreeHandler(
        IPermissionGroupQueryRepository _permissionGroupQueryRepository
    ) : IQueryHandler<GetPermissionGroupTreeQuery, Result<List<PermissionGroupTreeDto>>>
    {
        public async Task<Result<List<PermissionGroupTreeDto>>> HandleAsync(GetPermissionGroupTreeQuery request, CancellationToken cancellationToken)
        {
            var criteria = new PermissionGroupTreeCriteria
            {
                ParentId = request.ParentId,
                Name = request.Name
            };

            var tree = await _permissionGroupQueryRepository.GetPermissionGroupTreeAsync(criteria, cancellationToken);
            return Result<List<PermissionGroupTreeDto>>.Ok(tree);
        }
    }
}

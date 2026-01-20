using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取角色列表处理器
/// </summary>
public class GetRoleListHandler(IRoleQueryRepository _roleQueryRepository)
    : IQueryHandler<GetRoleListQuery, Result<List<RoleListDto>>>
{
    public async Task<Result<List<RoleListDto>>> HandleAsync(GetRoleListQuery query, CancellationToken cancellationToken)
    {
        var roles = await _roleQueryRepository.SearchAsync(query.Keyword, cancellationToken);
        return Result<List<RoleListDto>>.Ok(roles);
    }
}

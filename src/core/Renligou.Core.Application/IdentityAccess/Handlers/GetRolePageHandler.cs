using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取角色分页处理器
/// </summary>
public class GetRolePageHandler(IRoleQueryRepository _roleQueryRepository)
    : IQueryHandler<GetRolePageQuery, Result<Pagination<RoleDetailDto>>>
{
    public async Task<Result<Pagination<RoleDetailDto>>> HandleAsync(GetRolePageQuery query, CancellationToken cancellationToken)
    {
        var page = await _roleQueryRepository.PaginateAsync(query.Keyword, query.Page, query.PageSize, cancellationToken);
        return Result<Pagination<RoleDetailDto>>.Ok(page);
    }
}

using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取角色详情处理器
/// </summary>
public class GetRoleDetailHandler(IRoleQueryRepository _roleQueryRepository)
    : IQueryHandler<GetRoleDetailQuery, Result<RoleDetailDto?>>
{
    public async Task<Result<RoleDetailDto?>> HandleAsync(GetRoleDetailQuery query, CancellationToken cancellationToken)
    {
        var role = await _roleQueryRepository.QueryDetailAsync(query.RoleId, cancellationToken);
        if (role == null) return Result<RoleDetailDto?>.Fail("Role.NotFound", $"未找到角色: {query.RoleId}");
        return Result<RoleDetailDto?>.Ok(role);
    }
}

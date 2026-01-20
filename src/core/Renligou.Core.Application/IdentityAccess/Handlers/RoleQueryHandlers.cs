using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Repo;

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

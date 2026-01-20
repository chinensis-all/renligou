using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Repo;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取角色详情查询
/// </summary>
public sealed record GetRoleDetailQuery(long RoleId) : IQuery<Result<RoleDetailDto?>>;

/// <summary>
/// 获取角色列表查询
/// </summary>
public sealed record GetRoleListQuery(string? Keyword = null) : IQuery<Result<List<RoleListDto>>>;

/// <summary>
/// 获取角色分页查询
/// </summary>
public sealed record GetRolePageQuery(string? Keyword = null, int Page = 1, int PageSize = 20) : IQuery<Result<Pagination<RoleDetailDto>>>;

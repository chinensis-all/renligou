using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取角色分页查询
/// </summary>
public sealed record GetRolePageQuery(string? Keyword = null, int Page = 1, int PageSize = 20) : IQuery<Result<Pagination<RoleDetailDto>>>;

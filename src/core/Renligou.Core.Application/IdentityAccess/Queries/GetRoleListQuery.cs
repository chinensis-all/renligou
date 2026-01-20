using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取角色列表查询
/// </summary>
public sealed record GetRoleListQuery(string? Keyword = null) : IQuery<Result<List<RoleListDto>>>;

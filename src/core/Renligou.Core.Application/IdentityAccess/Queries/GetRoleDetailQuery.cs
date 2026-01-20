using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取角色详情查询
/// </summary>
public sealed record GetRoleDetailQuery(long RoleId) : IQuery<Result<RoleDetailDto?>>;

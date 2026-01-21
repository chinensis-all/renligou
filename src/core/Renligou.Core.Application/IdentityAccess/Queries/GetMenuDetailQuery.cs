using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取菜单详情查询
/// </summary>
public sealed record GetMenuDetailQuery(long Id) : IQuery<Result<MenuDetailDto?>>;

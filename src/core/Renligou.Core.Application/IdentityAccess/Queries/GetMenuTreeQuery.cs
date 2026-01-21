using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取菜单树查询
/// </summary>
public sealed record GetMenuTreeQuery(long ParentId = 0) : IQuery<Result<List<MenuTreeNodeDto>>>;

using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取菜单树处理器
/// </summary>
public sealed class GetMenuTreeHandler(
    IMenuQueryRepository _menuQueryRepository
) : IQueryHandler<GetMenuTreeQuery, Result<List<MenuTreeNodeDto>>>
{
    public async Task<Result<List<MenuTreeNodeDto>>> HandleAsync(GetMenuTreeQuery query, CancellationToken cancellationToken)
    {
        var tree = await _menuQueryRepository.GetMenuTreeAsync(query.ParentId, cancellationToken);
        return Result<List<MenuTreeNodeDto>>.Ok(tree);
    }
}

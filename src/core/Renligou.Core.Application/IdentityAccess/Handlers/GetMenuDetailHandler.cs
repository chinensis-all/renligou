using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取菜单详情处理器
/// </summary>
public sealed class GetMenuDetailHandler(
    IMenuQueryRepository _menuQueryRepository
) : IQueryHandler<GetMenuDetailQuery, Result<MenuDetailDto?>>
{
    public async Task<Result<MenuDetailDto?>> HandleAsync(GetMenuDetailQuery query, CancellationToken cancellationToken)
    {
        var detail = await _menuQueryRepository.QueryDetailAsync(query.Id, cancellationToken);
        return Result<MenuDetailDto?>.Ok(detail);
    }
}

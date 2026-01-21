using System.Collections.Generic;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Common;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 菜单查询仓储接口
/// </summary>
public interface IMenuQueryRepository : IRepository
{
    /// <summary>
    /// 获取菜单详情
    /// </summary>
    Task<MenuDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取菜单树
    /// </summary>
    Task<List<MenuTreeNodeDto>> GetMenuTreeAsync(long parentId = 0, CancellationToken cancellationToken = default);
}

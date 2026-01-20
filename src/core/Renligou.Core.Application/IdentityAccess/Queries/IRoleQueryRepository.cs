using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 角色查询仓储接口
/// </summary>
public interface IRoleQueryRepository : IRepository
{
    /// <summary>
    /// 获取角色详情
    /// </summary>
    Task<RoleDetailDto?> QueryDetailAsync(long roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索角色列表
    /// </summary>
    Task<List<RoleListDto>> SearchAsync(string? keyword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询角色
    /// </summary>
    Task<Pagination<RoleDetailDto>> PaginateAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken = default);
}

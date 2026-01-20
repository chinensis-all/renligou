using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries;

public interface IDepartmentQueryRepository : IRepository
{
    Task<DepartmentDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default);
    Task<List<DepartmentListDto>> SearchAsync(long? companyId, long? parentId, string? deptName, CancellationToken cancellationToken = default);
    Task<Pagination<DepartmentDetailDto>> PaginateAsync(int pageIndex, int pageSize, long? companyId, long? parentId, string? deptName, CancellationToken cancellationToken = default);
}

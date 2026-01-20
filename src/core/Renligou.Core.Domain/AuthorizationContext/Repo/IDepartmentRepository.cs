using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Repo;

public interface IDepartmentRepository : IRepository, DomainRepository<Department>
{
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> IsCompanyDeptNameConflictAsync(long companyId, long parentId, string deptName, CancellationToken cancellationToken = default);
}

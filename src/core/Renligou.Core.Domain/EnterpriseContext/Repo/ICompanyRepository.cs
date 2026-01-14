using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Domain.EnterpriseContext.Model;
using Renligou.Core.Shared.Common;

namespace Renligou.Core.Domain.EnterpriseContext.Repo
{
    public interface ICompanyRepository : IRepository, DomainRepository<Company>
    {
        Task<bool> CompanyNameExistsAsync(long companyId, string companyName);
    }
}

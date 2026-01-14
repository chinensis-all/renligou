using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.Enterprise.Criterias;
using Renligou.Core.Shared.Common;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public interface ICompanyQueryRepository : IRepository
    {
        Task<CompanyDetailDto?> QueryDetailAsync(long companyId, CancellationToken cancellationToken = default);

        Task<long> CountAsync(CompanySearchCriteria companyCriteria, CancellationToken cancellationToken = default);

        Task<List<CompanyListDto>> SearchAsync(CompanySearchCriteria companyCriteria, CancellationToken cancellationToken = default);

        Task<Pagination<CompanyDetailDto>> PaginateAsync(CompanySearchCriteria companyCriteria, CompanyPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default);
    }
}

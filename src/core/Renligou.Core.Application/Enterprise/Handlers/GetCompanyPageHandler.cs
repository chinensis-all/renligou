using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.Enterprise.Criterias;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class GetCompanyPageHandler(
        ICompanyQueryRepository _companyQueryRepository
    ) : IQueryHandler<GetCompanyPageQuery, Result<Pagination<CompanyDetailDto>>>
    {
        public async Task<Result<Pagination<CompanyDetailDto>>> HandleAsync(GetCompanyPageQuery query, CancellationToken cancellationToken)
        {
            var seachCriteria = new CompanySearchCriteria(
                CompanyType: query.CompanyType,
                CompanyName: query.CompanyName,
                ProvinceId: query.ProvinceId,
                Status: query.Status,
                Actived: query.Actived
            );

            var paginateCriteria = new CompanyPaginateCriteria();

            var pagination = await _companyQueryRepository.PaginateAsync(
                seachCriteria,
                paginateCriteria
            );

            return Result<Pagination<CompanyDetailDto>>.Ok(pagination);
        }
    }
}

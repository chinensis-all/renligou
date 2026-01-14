using Renligou.Core.Application.Enterprise.Criterias;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class GetCompanyListHandler(
        ICompanyQueryRepository _companyQueryRepository
    ) : IQueryHandler<GetCompanyListQuery, Result<List<CompanyListDto>>>
    {
        public async Task<Result<List<CompanyListDto>>> HandleAsync(GetCompanyListQuery query, CancellationToken cancellationToken)
        {
            var criteria = new CompanySearchCriteria(
                query.CompanyType,
                query.CompanyName,
                query.ProvinceId,
                query.Status,
                query.Actived
            );

            List<CompanyListDto> dtos = await _companyQueryRepository.SearchAsync(
                criteria,
                cancellationToken
            );

            return Result<List<CompanyListDto>>.Ok(dtos);
        }
    }
}

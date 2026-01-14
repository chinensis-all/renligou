using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Handlers
{
    public class GetCompanyDetailHandler(
        ICompanyQueryRepository _companyQueryRepository
    ) : IQueryHandler<GetCompanyDetailQuery, Result<CompanyDetailDto?>>
    {
        public async Task<Result<CompanyDetailDto?>> HandleAsync(GetCompanyDetailQuery query, CancellationToken cancellationToken)
        {
            CompanyDetailDto? dto = await _companyQueryRepository.QueryDetailAsync(
                query.CompanyId,
                cancellationToken
            );

            if (dto == null)
            {
                return Result<CompanyDetailDto?>.Fail("Company.NotFound", "未找到公司：" + query.CompanyId);
            }

            return Result<CompanyDetailDto?>.Ok(dto);
        }
    }
}

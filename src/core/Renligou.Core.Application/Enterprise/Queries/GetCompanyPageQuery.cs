using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record GetCompanyPageQuery(
        string? CompanyType,
        string? CompanyName,
        long? ProvinceId,
        string? Status,
        bool? Actived,
        int Page,
        int PageSize    
    ) : IQuery<Result<Pagination<CompanyDetailDto>>>
    {
    }
}

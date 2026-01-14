using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record GetCompanyListQuery(
        string? CompanyType,
        string? CompanyName,
        long? ProvinceId,
        string? Status,
        bool? Actived
    ) : IQuery<Result<List<CompanyListDto>>>
    {
    }
}

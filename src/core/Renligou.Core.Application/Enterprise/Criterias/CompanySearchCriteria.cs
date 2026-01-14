
namespace Renligou.Core.Application.Enterprise.Criterias
{
    public sealed record CompanySearchCriteria(
        string? CompanyType,
        string? CompanyName,
        long? ProvinceId,
        string? Status,
        bool? Actived
    )
    {
    }
}

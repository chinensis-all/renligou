
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record GetCompanyDetailQuery(long CompanyId) : IQuery<Result<CompanyDetailDto?>>
    {
    }
}

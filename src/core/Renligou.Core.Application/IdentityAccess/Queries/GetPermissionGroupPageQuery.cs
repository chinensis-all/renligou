using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    public sealed record GetPermissionGroupPageQuery(
        string? GroupName,
        string? DisplayName,
        int Page,
        int PageSize
    ) : IQuery<Result<Pagination<PermissionGroupDetailDto>>>;
}

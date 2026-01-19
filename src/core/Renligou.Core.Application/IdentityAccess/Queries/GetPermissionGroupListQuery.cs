using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    public sealed record GetPermissionGroupListQuery(string? GroupName, string? DisplayName) : IQuery<Result<List<PermissionGroupListDto>>>;
}

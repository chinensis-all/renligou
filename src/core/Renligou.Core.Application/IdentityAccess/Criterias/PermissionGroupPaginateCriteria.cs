namespace Renligou.Core.Application.IdentityAccess.Criterias
{
    public record PermissionGroupPaginateCriteria(
        int Page = 1,
        int PageSize = 10
    );
}

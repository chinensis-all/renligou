namespace Renligou.Core.Application.IdentityAccess.Criterias
{
    public record PermissionGroupSearchCriteria(
        string? GroupName = null,
        string? DisplayName = null
    );
}

namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 权限列表DTO
    /// </summary>
    public sealed record PermissionListDto
    {
        public string Id { get; init; } = string.Empty;

        public string GroupId { get; init; } = string.Empty;

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;
    }
}

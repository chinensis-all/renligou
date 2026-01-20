namespace Renligou.Core.Application.IdentityAccess.Queries
{
    /// <summary>
    /// 权限详情DTO
    /// </summary>
    public sealed record PermissionDetailDto
    {
        public string Id { get; init; } = string.Empty;

        public string GroupId { get; init; } = string.Empty;

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string CreatedAt { get; init; } = string.Empty;

        public string UpdatedAt { get; init; } = string.Empty;
    }
}

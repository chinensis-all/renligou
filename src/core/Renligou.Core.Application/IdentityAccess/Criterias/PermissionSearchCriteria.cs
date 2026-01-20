namespace Renligou.Core.Application.IdentityAccess.Criterias
{
    /// <summary>
    /// 权限查询条件
    /// </summary>
    public sealed record PermissionSearchCriteria
    {
        public long? GroupId { get; init; }

        public string? PermissionName { get; init; }

        public string? DisplayName { get; init; }
    }
}

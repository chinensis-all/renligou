namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 角色详情 DTO
/// </summary>
public record RoleDetailDto
{
    public long Id { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}

/// <summary>
/// 角色列表 DTO
/// </summary>
public record RoleListDto
{
    public long Id { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;
}

using Renligou.Core.Shared.Events;

namespace Renligou.Core.AuthorizationContext.Event;

/// <summary>
/// 角色已修改领域事件
/// </summary>
public sealed record RoleModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }

    public long Id { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; }
}

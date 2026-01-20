using Renligou.Core.Shared.Events;

namespace Renligou.Core.AuthorizationContext.Event;

/// <summary>
/// 角色已删除(软删除)领域事件
/// </summary>
public sealed record RoleDestroyedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }

    public long Id { get; init; }
}

using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event;

/// <summary>
/// 菜单销毁领域事件
/// </summary>
public sealed record MenuDestroyedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long Id { get; init; }
}

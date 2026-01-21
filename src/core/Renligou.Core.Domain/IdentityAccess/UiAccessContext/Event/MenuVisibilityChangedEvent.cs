using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event;

/// <summary>
/// 菜单显示状态变更领域事件
/// </summary>
public sealed record MenuVisibilityChangedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long Id { get; init; }
    public bool IsHidden { get; init; }
}

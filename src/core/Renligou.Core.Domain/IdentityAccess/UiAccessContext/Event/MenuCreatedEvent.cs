using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event;

/// <summary>
/// 菜单创建领域事件
/// </summary>
public sealed record MenuCreatedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long Id { get; init; }
    public long ParentId { get; init; }
    public string MenuName { get; init; } = string.Empty;
    public string MenuTag { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string Component { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public int Sorter { get; init; }
    public bool IsHidden { get; init; }
    public string PermitButtons { get; init; } = string.Empty;
}

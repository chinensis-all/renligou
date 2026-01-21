using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

/// <summary>
/// 部门状态修改事件
/// </summary>
public sealed record DepartmentStatusModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    public long Id { get; init; }

    public string Status { get; init; } = string.Empty;
}

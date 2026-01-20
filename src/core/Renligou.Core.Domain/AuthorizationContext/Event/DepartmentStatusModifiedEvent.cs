using Renligou.Core.Shared.Events;
using Renligou.Core.Domain.AuthorizationContext.Value;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

public sealed record DepartmentStatusModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long DepartmentId { get; init; }
    public DepartmentStatus Status { get; init; } = default!;
}

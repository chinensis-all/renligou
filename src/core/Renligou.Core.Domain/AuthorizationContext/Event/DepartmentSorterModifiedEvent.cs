using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

public sealed record DepartmentSorterModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long DepartmentId { get; init; }
    public int Sorter { get; init; }
}

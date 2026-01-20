using Renligou.Core.Shared.Events;
using Renligou.Core.Domain.AuthorizationContext.Value;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

public sealed record DepartmentCreatedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long Id { get; init; }
    public long ParentId { get; init; }
    public long CompanyId { get; init; }
    public string DeptName { get; init; } = string.Empty;
    public string DeptCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Sorter { get; init; }
    public DepartmentStatus Status { get; init; } = default!;
}

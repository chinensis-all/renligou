using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

public sealed record DepartmentBasicModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }
    public long DepartmentId { get; init; }
    public long ParentId { get; init; }
    public long CompanyId { get; init; }
    public string DeptName { get; init; } = string.Empty;
    public string DeptCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

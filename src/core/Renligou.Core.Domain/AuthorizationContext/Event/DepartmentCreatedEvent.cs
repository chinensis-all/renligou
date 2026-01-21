using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

/// <summary>
/// 部门创建事件
/// </summary>
public sealed record DepartmentCreatedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    public long Id { get; init; }

    public long ParentId { get; init; }

    public long CompanyId { get; init; }

    public string DeptName { get; init; } = string.Empty;

    public string DeptCode { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Sorter { get; init; }

    public string Status { get; init; } = string.Empty;
}

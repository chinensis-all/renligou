using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event;

/// <summary>
/// 部门修改事件
/// </summary>
public sealed record DepartmentModifiedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    public long Id { get; init; }

    public long ParentId { get; init; }

    public string DeptName { get; init; } = string.Empty;

    public string DeptCode { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Sorter { get; init; }
}

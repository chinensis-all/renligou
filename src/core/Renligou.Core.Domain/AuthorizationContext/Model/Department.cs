using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model;

public class Department : AggregateBase
{
    public long ParentId { get; private set; }
    public long CompanyId { get; private set; }
    public string DeptName { get; private set; } = string.Empty;
    public string DeptCode { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Sorter { get; private set; }
    public DepartmentStatus Status { get; private set; } = default!;

    public Department(
        AggregateId id,
        long parentId,
        long companyId,
        string deptName,
        string deptCode,
        string description,
        int sorter,
        DepartmentStatus status
    )
    {
        Id = id;
        ParentId = parentId;
        CompanyId = companyId;
        DeptName = deptName;
        DeptCode = deptCode;
        Description = description;
        Sorter = sorter;
        Status = status;
    }

    public void Create()
    {
        var @event = new DepartmentCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            ParentId = ParentId,
            CompanyId = CompanyId,
            DeptName = DeptName,
            DeptCode = DeptCode,
            Description = Description,
            Sorter = Sorter,
            Status = Status
        };
        RegisterEvent(@event);
    }

    public void ModifyBasic(
        long parentId,
        long companyId,
        string deptName,
        string deptCode,
        string description
    )
    {
        ParentId = parentId;
        CompanyId = companyId;
        DeptName = deptName;
        DeptCode = deptCode;
        Description = description;

        var @event = new DepartmentBasicModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            DepartmentId = Id.id,
            ParentId = ParentId,
            CompanyId = CompanyId,
            DeptName = DeptName,
            DeptCode = DeptCode,
            Description = Description
        };
        RegisterEvent(@event);
    }

    public void ModifySorter(int sorter)
    {
        Sorter = sorter;

        var @event = new DepartmentSorterModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            DepartmentId = Id.id,
            Sorter = Sorter
        };
        RegisterEvent(@event);
    }

    public void Activate()
    {
        if (Status == DepartmentStatus.Active) return;
        Status = DepartmentStatus.Active;

        var @event = new DepartmentStatusModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            DepartmentId = Id.id,
            Status = Status
        };
        RegisterEvent(@event);
    }

    public void Inactivate()
    {
        if (Status == DepartmentStatus.Inactive) return;
        Status = DepartmentStatus.Inactive;

        var @event = new DepartmentStatusModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            DepartmentId = Id.id,
            Status = Status
        };
        RegisterEvent(@event);
    }
}

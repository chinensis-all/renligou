using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model;

/// <summary>
/// 部门聚合根
/// </summary>
public class Department : AggregateBase
{
    /// <summary>
    /// 上级部门ID
    /// </summary>
    public long ParentId { get; private set; }

    /// <summary>
    /// 所属公司ID
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; private set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string DeptCode { get; private set; } = string.Empty;

    /// <summary>
    /// 部门描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sorter { get; private set; }

    /// <summary>
    /// 部门状态
    /// </summary>
    public DepartmentStatus Status { get; private set; } = DepartmentStatus.Active;

    /// <summary>
    /// 私有构造函数，用于仓储加载
    /// </summary>
    private Department() { }

    /// <summary>
    /// 用于创建新部门的构造函数
    /// </summary>
    public Department(
        AggregateId id,
        long parentId,
        long companyId,
        string deptName,
        string deptCode,
        string description,
        int sorter)
    {
        Id = id;
        ParentId = parentId;
        CompanyId = companyId;
        DeptName = deptName;
        DeptCode = deptCode;
        Description = description;
        Sorter = sorter;
        Status = DepartmentStatus.Active;
    }

    /// <summary>
    /// 创建部门并注册领域事件
    /// </summary>
    public void Create()
    {
        RegisterEvent(new DepartmentCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            ParentId = ParentId,
            CompanyId = CompanyId,
            DeptName = DeptName,
            DeptCode = DeptCode,
            Description = Description,
            Sorter = Sorter,
            Status = Status.Code
        });
    }

    /// <summary>
    /// 修改部门基础信息
    /// </summary>
    public void ModifyBasic(long parentId, string deptName, string deptCode, string description, int sorter)
    {
        ParentId = parentId;
        DeptName = deptName;
        DeptCode = deptCode;
        Description = description;
        Sorter = sorter;

        RegisterEvent(new DepartmentModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            ParentId = ParentId,
            DeptName = DeptName,
            DeptCode = DeptCode,
            Description = Description,
            Sorter = Sorter
        });
    }

    /// <summary>
    /// 禁用部门
    /// </summary>
    public void Inactive()
    {
        if (Status == DepartmentStatus.Inactive) return;

        Status = DepartmentStatus.Inactive;

        RegisterEvent(new DepartmentStatusModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            Status = Status.Code
        });
    }

    /// <summary>
    /// 启用部门
    /// </summary>
    public void Activate()
    {
        if (Status == DepartmentStatus.Active) return;

        Status = DepartmentStatus.Active;

        RegisterEvent(new DepartmentStatusModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            Status = Status.Code
        });
    }
}

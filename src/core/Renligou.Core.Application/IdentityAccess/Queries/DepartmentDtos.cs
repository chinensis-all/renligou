using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

public sealed record DepartmentDetailDto
{
    [Description("部门ID")]
    public string Id { get; init; } = string.Empty;

    [Description("上级部门ID")]
    public string ParentId { get; init; } = string.Empty;

    [Description("所属公司ID")]
    public string CompanyId { get; init; } = string.Empty;

    [Description("部门名称")]
    public string DeptName { get; init; } = string.Empty;

    [Description("部门编码")]
    public string DeptCode { get; init; } = string.Empty;

    [Description("描述")]
    public string Description { get; init; } = string.Empty;

    [Description("排序")]
    public int Sorter { get; init; }

    [Description("状态")]
    public string Status { get; init; } = string.Empty;

    [Description("创建时间")]
    public DateTime CreatedAt { get; init; }

    [Description("更新时间")]
    public DateTime UpdatedAt { get; init; }
}

public sealed record DepartmentListDto
{
    [Description("部门ID")]
    public string Id { get; init; } = string.Empty;

    [Description("上级部门ID")]
    public string ParentId { get; init; } = string.Empty;

    [Description("部门名称")]
    public string DeptName { get; init; } = string.Empty;

    [Description("部门编码")]
    public string DeptCode { get; init; } = string.Empty;

    [Description("排序")]
    public int Sorter { get; init; }

    [Description("状态")]
    public string Status { get; init; } = string.Empty;
}

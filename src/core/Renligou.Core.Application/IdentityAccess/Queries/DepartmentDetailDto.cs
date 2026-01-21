using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 部门详情 DTO
/// </summary>
public sealed record DepartmentDetailDto
{
    [Description("部门ID")]
    public long Id { get; init; }

    [Description("上级部门ID")]
    public long ParentId { get; init; }

    [Description("所属公司ID")]
    public long CompanyId { get; init; }

    [Description("部门名称")]
    public string DeptName { get; init; } = string.Empty;

    [Description("部门编码")]
    public string DeptCode { get; init; } = string.Empty;

    [Description("部门描述")]
    public string Description { get; init; } = string.Empty;

    [Description("排序号")]
    public int Sorter { get; init; }

    [Description("部门状态")]
    public string Status { get; init; } = string.Empty;

    [Description("创建时间")]
    public DateTimeOffset CreatedAt { get; init; }
}

using System.ComponentModel;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 部门列表 DTO
/// </summary>
public sealed record DepartmentListDto
{
    [Description("部门ID")]
    public long Id { get; init; }

    [Description("部门名称")]
    public string DeptName { get; init; } = string.Empty;

    [Description("部门编码")]
    public string DeptCode { get; init; } = string.Empty;

    [Description("部门状态")]
    public string Status { get; init; } = string.Empty;

    [Description("排序号")]
    public int Sorter { get; init; }
}

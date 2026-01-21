using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests;

/// <summary>
/// 修改部门基础信息请求
/// </summary>
public sealed record ModifyDepartmentBasicRequest
{
    [Description("上级部门ID")]
    public long ParentId { get; init; }

    [Description("部门名称")]
    [Required(ErrorMessage = "部门名称不能为空")]
    [MaxLength(100, ErrorMessage = "部门名称不能超过100个字符")]
    public string DeptName { get; init; } = string.Empty;

    [Description("部门编码")]
    [MaxLength(30, ErrorMessage = "部门编码不能超过30个字符")]
    public string DeptCode { get; init; } = string.Empty;

    [Description("部门描述")]
    [MaxLength(500, ErrorMessage = "部门描述不能超过500个字符")]
    public string Description { get; init; } = string.Empty;

    [Description("排序号")]
    public int Sorter { get; init; }
}

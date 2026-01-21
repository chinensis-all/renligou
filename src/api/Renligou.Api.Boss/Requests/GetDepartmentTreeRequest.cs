using System.ComponentModel;

namespace Renligou.Api.Boss.Requests;

/// <summary>
/// 获取部门树请求
/// </summary>
public sealed record GetDepartmentTreeRequest
{
    [Description("父节点ID")]
    public long ParentId { get; init; }

    [Description("部门名称关键字")]
    public string? Name { get; init; }

    [Description("所属公司ID")]
    public long? CompanyId { get; init; }
}

namespace Renligou.Api.Boss.Requests;

public class ModifyDepartmentBasicRequest
{
    public long ParentId { get; set; }
    public long CompanyId { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string DeptCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

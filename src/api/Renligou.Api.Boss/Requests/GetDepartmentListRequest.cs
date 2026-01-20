namespace Renligou.Api.Boss.Requests;

public class GetDepartmentListRequest
{
    public long? CompanyId { get; set; }
    public long? ParentId { get; set; }
    public string? DeptName { get; set; }
}

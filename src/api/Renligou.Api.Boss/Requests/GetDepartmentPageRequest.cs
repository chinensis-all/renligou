namespace Renligou.Api.Boss.Requests;

public class GetDepartmentPageRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public long? CompanyId { get; set; }
    public long? ParentId { get; set; }
    public string? DeptName { get; set; }
}

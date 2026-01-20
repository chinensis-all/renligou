using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

public sealed record CreateDepartmentCommand : ICommand<Result>
{
    public long ParentId { get; init; }
    public long CompanyId { get; init; }
    public string DeptName { get; init; } = string.Empty;
    public string DeptCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Sorter { get; init; }

    public Result Validate()
    {
        if (CompanyId <= 0)
            return Result.Fail("Department.Create.Error", "缺失所属公司ID");

        if (string.IsNullOrEmpty(DeptName))
            return Result.Fail("Department.Create.Error", "缺失部门名称");

        return Result.Ok();
    }
}

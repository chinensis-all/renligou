using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

public sealed record ModifyDepartmentBasicCommand : ICommand<Result>
{
    public long Id { get; init; }
    public long ParentId { get; init; }
    public long CompanyId { get; init; }
    public string DeptName { get; init; } = string.Empty;
    public string DeptCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public Result Validate()
    {
        if (Id <= 0)
            return Result.Fail("Department.Modify.Error", "无效的部门ID");

        if (CompanyId <= 0)
            return Result.Fail("Department.Modify.Error", "缺失所属公司ID");

        if (string.IsNullOrEmpty(DeptName))
            return Result.Fail("Department.Modify.Error", "缺失部门名称");

        return Result.Ok();
    }
}

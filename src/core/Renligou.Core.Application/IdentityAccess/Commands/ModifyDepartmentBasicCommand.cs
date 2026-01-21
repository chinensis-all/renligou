using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 修改部门基础信息命令
/// </summary>
public sealed record ModifyDepartmentBasicCommand : ICommand<Result>
{
    public long Id { get; init; }
    public long ParentId { get; init; }
    public string DeptName { get; init; } = string.Empty;
    public string DeptCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Sorter { get; init; }

    public Result Validate()
    {
        if (Id <= 0)
        {
            return Result.Fail("Department.Modify.Error", "部门ID必须大于0");
        }

        if (string.IsNullOrWhiteSpace(DeptName))
        {
            return Result.Fail("Department.Modify.Error", "部门名称不能为空");
        }

        return Result.Ok();
    }
}

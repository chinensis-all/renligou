using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 创建部门命令
/// </summary>
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
        if (string.IsNullOrWhiteSpace(DeptName))
        {
            return Result.Fail("Department.Create.Error", "部门名称不能为空");
        }

        if (CompanyId <= 0)
        {
            return Result.Fail("Department.Create.Error", "公司ID必须大于0");
        }

        return Result.Ok();
    }
}

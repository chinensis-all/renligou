using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

public sealed record ModifyDepartmentSorterCommand : ICommand<Result>
{
    public long Id { get; init; }
    public int Sorter { get; init; }

    public Result Validate()
    {
        if (Id <= 0)
            return Result.Fail("Department.ModifySorter.Error", "无效的部门ID");
        return Result.Ok();
    }
}

using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 启用部门命令
/// </summary>
public sealed record ActivateDepartmentCommand(long Id) : ICommand<Result>
{
    public Result Validate()
    {
        if (Id <= 0)
        {
            return Result.Fail("Department.Activate.Error", "部门ID必须大于0");
        }

        return Result.Ok();
    }
}

using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 销毁(软删除)角色命令
/// </summary>
public sealed record DestroyRoleCommand(long RoleId) : ICommand<Result>
{
    public Result Validate()
    {
        if (RoleId <= 0)
        {
            return Result.Fail("Role.Destroy.Error", "非法的角色ID");
        }

        return Result.Ok();
    }
}

using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 修改菜单显示状态命令
/// </summary>
public sealed record ChangeMenuVisibilityCommand(long Id, bool IsHidden) : ICommand<Result>
{
    public Result Validate()
    {
        if (Id <= 0) return Result.Fail("Menu.Visibility.Error", "无效的菜单ID");
        return Result.Ok();
    }
}

using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 创建角色命令
/// </summary>
public sealed record CreateRoleCommand : ICommand<Result>
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; init; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(RoleName))
        {
            return Result.Fail("Role.Create.Error", "角色名称不能为空");
        }

        if (string.IsNullOrWhiteSpace(DisplayName))
        {
            return Result.Fail("Role.Create.Error", "显示名称不能为空");
        }

        return Result.Ok();
    }
}

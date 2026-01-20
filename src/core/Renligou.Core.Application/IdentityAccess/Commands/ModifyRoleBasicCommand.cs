using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 修改角色基础信息命令
/// </summary>
public sealed record ModifyRoleBasicCommand : ICommand<Result>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; init; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; init; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; init; } = string.Empty;

    public Result Validate()
    {
        if (RoleId <= 0)
        {
            return Result.Fail("Role.Modify.Error", "非法的角色ID");
        }

        if (string.IsNullOrWhiteSpace(RoleName))
        {
            return Result.Fail("Role.Modify.Error", "角色名称不能为空");
        }

        if (string.IsNullOrWhiteSpace(DisplayName))
        {
            return Result.Fail("Role.Modify.Error", "显示名称不能为空");
        }

        return Result.Ok();
    }
}

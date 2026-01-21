using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 修改菜单命令
/// </summary>
public sealed record ModifyMenuCommand : ICommand<Result>
{
    public long Id { get; init; }
    public long ParentId { get; init; }
    public string MenuName { get; init; } = string.Empty;
    public string MenuTag { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string Component { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public int Sorter { get; init; }
    public string PermitButtons { get; init; } = string.Empty;

    public Result Validate()
    {
        if (Id <= 0) return Result.Fail("Menu.Modify.Error", "无效的菜单ID");
        if (string.IsNullOrWhiteSpace(MenuName)) return Result.Fail("Menu.Modify.Error", "菜单名称不能为空");
        return Result.Ok();
    }
}

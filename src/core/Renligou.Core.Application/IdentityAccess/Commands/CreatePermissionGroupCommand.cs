using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    public sealed record CreatePermissionGroupCommand : ICommand<Result>
    {
        public string GroupName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public long ParentId { get; init; }

        public int Sorter { get; init; }

        public Result Validate()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                return Result.Fail("PermissionGroup.Create.Error", "权限组名称不能为空");
            }

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                return Result.Fail("PermissionGroup.Create.Error", "权限组显示名称不能为空");
            }

            return Result.Ok();
        }
    }
}

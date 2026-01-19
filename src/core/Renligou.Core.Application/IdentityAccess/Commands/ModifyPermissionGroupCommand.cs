using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    public sealed record ModifyPermissionGroupCommand : ICommand<Result>
    {
        public long Id { get; init; }

        public string GroupName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public Result Validate()
        {
             if (string.IsNullOrWhiteSpace(GroupName))
            {
                return Result.Fail("PermissionGroup.Modify.Error", "权限组名称不能为空");
            }

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                return Result.Fail("PermissionGroup.Modify.Error", "权限组显示名称不能为空");
            }
            return Result.Ok();
        }
    }
}

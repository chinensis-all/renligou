using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    /// <summary>
    /// 修改权限命令
    /// </summary>
    public sealed record ModifyPermissionCommand : ICommand<Result>
    {
        public long Id { get; init; }

        public long GroupId { get; init; }

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public Result Validate()
        {
            if (Id <= 0)
            {
                return Result.Fail("Permission.Modify.Error", "缺失权限ID");
            }

            if (GroupId <= 0)
            {
                return Result.Fail("Permission.Modify.Error", "缺失权限组ID");
            }

            if (string.IsNullOrEmpty(PermissionName))
            {
                return Result.Fail("Permission.Modify.Error", "缺失权限标识");
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                return Result.Fail("Permission.Modify.Error", "缺失权限名称");
            }

            return Result.Ok();
        }
    }
}

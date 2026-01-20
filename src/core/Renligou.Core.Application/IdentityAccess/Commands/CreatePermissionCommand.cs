using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    /// <summary>
    /// 创建权限命令
    /// </summary>
    public sealed record CreatePermissionCommand : ICommand<Result>
    {
        public long GroupId { get; init; }

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public Result Validate()
        {
            if (GroupId <= 0)
            {
                return Result.Fail("Permission.Create.Error", "缺失权限组ID");
            }

            if (string.IsNullOrEmpty(PermissionName))
            {
                return Result.Fail("Permission.Create.Error", "缺失权限标识");
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                return Result.Fail("Permission.Create.Error", "缺失权限名称");
            }

            return Result.Ok();
        }
    }
}

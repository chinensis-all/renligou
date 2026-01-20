using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    /// <summary>
    /// 删除权限命令
    /// </summary>
    public sealed record DestroyPermissionCommand(long Id) : ICommand<Result>;
}

using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands
{
    public sealed record DestroyPermissionGroupCommand(long Id) : ICommand<Result>;
}

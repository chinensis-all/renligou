using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Commands;

/// <summary>
/// 销毁菜单命令
/// </summary>
public sealed record DestroyMenuCommand(long Id) : ICommand<Result>;

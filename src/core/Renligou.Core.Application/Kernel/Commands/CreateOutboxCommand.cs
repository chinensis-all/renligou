using Renligou.Core.Shared.Commanding;

namespace Renligou.Core.Application.Kernel.Commands
{
    /// <summary>
    /// 记录当前线程事件命令
    /// </summary>
    public sealed record CreateOutboxCommand() : ICommand
    {
    }
}

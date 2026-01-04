using Renligou.Core.Shared.Commanding;

/// <summary>
/// 命令总线接口。
/// </summary>
namespace Renligou.Core.Shared.Bus;

/// <summary>
/// 提供命令派发入口。
/// </summary>
public interface ICommandBus
{
    /// <summary>
    /// 派发不带返回值的命令。
    /// </summary>
    /// <typeparam name="TCommand">命令类型。</typeparam>
    /// <param name="command">命令实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步任务。</returns>
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// 派发带返回值的命令。
    /// </summary>
    /// <typeparam name="TCommand">命令类型。</typeparam>
    /// <typeparam name="TResult">返回结果类型。</typeparam>
    /// <param name="command">命令实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带结果的任务。</returns>
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}

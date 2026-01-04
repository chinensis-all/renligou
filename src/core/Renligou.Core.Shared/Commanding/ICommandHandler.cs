/// <summary>
/// 命令处理器接口定义。
/// </summary>
namespace Renligou.Core.Shared.Commanding;

/// <summary>
/// 定义不带返回值的命令处理器。
/// </summary>
/// <typeparam name="TCommand">命令类型。</typeparam>
public interface ICommandHandler<in TCommand> 
    where TCommand : ICommand
{
    /// <summary>
    /// 处理命令。
    /// </summary>
    /// <param name="command">命令实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步任务。</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// 定义带返回值的命令处理器。
/// </summary>
/// <typeparam name="TCommand">命令类型。</typeparam>
/// <typeparam name="TResult">返回结果类型。</typeparam>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// 处理命令。
    /// </summary>
    /// <param name="command">命令实例。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带结果的任务。</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

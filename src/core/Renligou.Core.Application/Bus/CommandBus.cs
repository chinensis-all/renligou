using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection; 
using Renligou.Core.Application.Bus.Internal;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Exceptions;

/// <summary>
/// 命令总线实现。
/// </summary>
namespace Renligou.Core.Application.Bus;

/// <summary>
/// 高性能命令总线，通过缓存派发委托实现。
/// </summary>
public sealed class CommandBus : ICommandBus
{
    private readonly IServiceProvider _serviceProvider;
    
    // 缓存派发委托：(RequestType, ResponseType) -> 派发函数
    // 派发函数签名：(服务提供者, 请求实例, 取消令牌) -> 结果任务
    private static readonly ConcurrentDictionary<CacheKey, Func<IServiceProvider, object, CancellationToken, Task<object?>>> _delegateCache = new();

    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand
    {
        var key = new CacheKey(typeof(TCommand), typeof(object)); // 无返回值默认 object/void 占位
        var handlerDelegate = _delegateCache.GetOrAdd(key, k => CreateCommandDelegate<TCommand>());
        
        return handlerDelegate(_serviceProvider, command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand<TResult>
    {
        var key = new CacheKey(typeof(TCommand), typeof(TResult));
        var handlerDelegate = _delegateCache.GetOrAdd(key, k => CreateCommandDelegateWithResult<TCommand, TResult>());

        var result = await handlerDelegate(_serviceProvider, command, cancellationToken);
        return (TResult)result!;
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<object?>> CreateCommandDelegate<TCommand>()
        where TCommand : ICommand
    {
        // 核心优化：仅在此执行一次反射
        return async (sp, cmd, ct) =>
        {
            var handlers = sp.GetServices<ICommandHandler<TCommand>>();
            var handler = ValidateAndGetSingleHandler(handlers, typeof(TCommand));

            // 调用管道
            await PipelineInvoker.InvokeAsync<TCommand, object>(sp, (TCommand)cmd, ct, async () =>
            {
                await handler.HandleAsync((TCommand)cmd, ct);
                return null!;
            });

            return null;
        };
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<object?>> CreateCommandDelegateWithResult<TCommand, TResult>()
        where TCommand : ICommand<TResult>
    {
        return async (sp, cmd, ct) =>
        {
            var handlers = sp.GetServices<ICommandHandler<TCommand, TResult>>();
            var handler = ValidateAndGetSingleHandler(handlers, typeof(TCommand));

            var result = await PipelineInvoker.InvokeAsync<TCommand, TResult>(sp, (TCommand)cmd, ct, () => 
                handler.HandleAsync((TCommand)cmd, ct));
            
            return result;
        };
    }

    private static THandler ValidateAndGetSingleHandler<THandler>(IEnumerable<THandler> handlers, Type requestType)
    {
        THandler? found = default;
        int count = 0;

        foreach (var h in handlers)
        {
            found = h;
            count++;
            if (count > 1) break;
        }

        if (count == 0) throw new HandlerNotFoundException(requestType);
        if (count > 1) throw new MultipleHandlersFoundException(requestType);

        return found!;
    }
}

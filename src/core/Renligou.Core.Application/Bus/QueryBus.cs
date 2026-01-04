using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus.Internal;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Exceptions;
using Renligou.Core.Shared.Querying;

/// <summary>
/// 查询总线实现。
/// </summary>
namespace Renligou.Core.Application.Bus;

/// <summary>
/// 高性能查询总线，通过缓存派发委托实现。
/// </summary>
public sealed class QueryBus : IQueryBus
{
    private readonly IServiceProvider _serviceProvider;
    
    // 缓存派发委托：(RequestType, ResponseType) -> 派发函数
    private static readonly ConcurrentDictionary<CacheKey, Func<IServiceProvider, object, CancellationToken, Task<object?>>> _delegateCache = new();

    public QueryBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>
    {
        var key = new CacheKey(typeof(TQuery), typeof(TResult));
        var handlerDelegate = _delegateCache.GetOrAdd(key, k => CreateQueryDelegate<TQuery, TResult>());

        var result = await handlerDelegate(_serviceProvider, query, cancellationToken);
        return (TResult)result!;
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<object?>> CreateQueryDelegate<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        return async (sp, qry, ct) =>
        {
            var handlers = sp.GetServices<IQueryHandler<TQuery, TResult>>();
            var handler = ValidateAndGetSingleHandler(handlers, typeof(TQuery));

            // 调用管道
            var result = await PipelineInvoker.InvokeAsync<TQuery, TResult>(sp, (TQuery)qry, ct, () => 
                handler.HandleAsync((TQuery)qry, ct));

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

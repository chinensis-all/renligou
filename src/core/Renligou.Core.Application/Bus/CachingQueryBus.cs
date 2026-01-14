using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Cache;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Bus
{
    public class CachingQueryBus(
        IQueryBus _inner,
        ICache _cache
    ) : IQueryBus
    {
        public async Task<TResult> QueryAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken ct)
            where TQuery : IQuery<TResult>
        {
            if (query is not ICacheableQuery cacheable)
            {
                return await _inner.QueryAsync<TQuery, TResult>(query, ct);
            }

            var key = cacheable.GetCacheKey();

            if (_cache.TryGet<TResult>(key, out var cached))
            {
                return cached;
            }

            var result = await _inner.QueryAsync<TQuery, TResult>(query, ct);

            _cache.Set(key, result, cacheable.GetTtl());

            return result;
        }
    }
}

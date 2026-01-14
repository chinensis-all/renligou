using Renligou.Core.Shared.Cache;

namespace Renligou.Core.Infrastructure.Cache
{
    public class NoopCacheAdapter : ICache
    {
        public bool TryGet<T>(string key, out T value)
        {
            value = default!;
            return false;
        }

        public Task<bool> TryGetAsync<T>(string key, CancellationToken ct = default)
        {
            return Task.FromResult(false);
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            // no-op
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            // no-op
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public void RemoveByPrefix(string prefix)
        {
            // no-op
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}

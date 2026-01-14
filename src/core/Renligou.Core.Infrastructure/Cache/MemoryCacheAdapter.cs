using Microsoft.Extensions.Caching.Memory;
using Renligou.Core.Shared.Cache;
using System.Collections.Concurrent;

namespace Renligou.Core.Infrastructure.Cache
{
    public class MemoryCacheAdapter(
        IMemoryCache _cache
    ) : ICache
    {
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value!);
        }

        public Task<bool> TryGetAsync<T>(string key, CancellationToken ct = default)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
            _keys.TryAdd(key, 0);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
        {
            Set(key, value, ttl);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void RemoveByPrefix(string prefix)
        {
            var keys = _keys.Keys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            RemoveByPrefix(prefix);
            return Task.CompletedTask;
        }
    }
}

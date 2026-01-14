using Renligou.Core.Shared.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace Renligou.Core.Infrastructure.Cache
{
    public class RedisCacheAdapter : ICache
    {
        private readonly IDatabase _db;
        private readonly IServer _server;

        public RedisCacheAdapter(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
            _server = mux.GetServer(mux.GetEndPoints().First());
        }

        public bool TryGet<T>(string key, out T value)
        {
            var val = _db.StringGet(key);
            if (!val.HasValue)
            {
                value = default!;
                return false;
            }

            value = JsonSerializer.Deserialize<T>(val.ToString())!;
            return true;
        }

        public async Task<bool> TryGetAsync<T>(string key, CancellationToken ct = default)
        {
            return await _db.KeyExistsAsync(key);
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            var json = JsonSerializer.Serialize(value);
            _db.StringSet(key, json, ttl);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
        {
            Set(key, value, ttl);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void RemoveByPrefix(string prefix)
        {
            foreach (var key in _server.Keys(pattern: $"{prefix}*"))
            {
                _db.KeyDelete(key);
            }
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            RemoveByPrefix(prefix);
            return Task.CompletedTask;
        }
    }
}

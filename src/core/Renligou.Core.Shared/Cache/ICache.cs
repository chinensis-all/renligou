namespace Renligou.Core.Shared.Cache
{
    public interface ICache
    {
        bool TryGet<T>(string key, out T value);

        Task<bool> TryGetAsync<T>(string key, CancellationToken ct = default);

        void Set<T>(string key, T value, TimeSpan ttl);

        Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken ct = default);

        void RemoveByPrefix(string prefix);

        Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
    }
}

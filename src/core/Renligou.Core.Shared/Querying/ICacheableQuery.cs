namespace Renligou.Core.Shared.Querying
{
    public interface ICacheableQuery
    {
        string GetCacheKey();
        TimeSpan GetTtl();
    }
}

namespace Renligou.Core.Shared.Bus
{
    public interface IQueryMetrics
    {
        void Increment(string queryName);

        void ObserveDuration(string queryName, double milliseconds);

        void IncrementFailure(string queryName);
    }
}

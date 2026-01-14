namespace Renligou.Core.Infrastructure.External.Metrics
{
    public sealed record QueryMetricSnapshot(
        string QueryName,
        long TotalCount,
        long FailureCount,
        double TotalDurationMs
    )
    {
        public static QueryMetricSnapshot Empty(string name) => new(name, 0, 0, 0);
    }
}

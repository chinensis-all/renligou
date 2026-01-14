using Renligou.Core.Shared.Bus;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Renligou.Core.Infrastructure.External.Metrics
{
    public class InMemoryQueryMetrics : IQueryMetrics
    {
        private sealed class Metric
        {
            public long TotalCount;
            public long FailureCount;
            public double TotalDurationMs;
        }

        private readonly ConcurrentDictionary<string, Metric> _metrics = new();

        public void Increment(string queryName)
        {
            var metric = _metrics.GetOrAdd(queryName, _ => new Metric());
            Interlocked.Increment(ref metric.TotalCount);
        }

        public void IncrementFailure(string queryName)
        {
            var metric = _metrics.GetOrAdd(queryName, _ => new Metric());
            Interlocked.Increment(ref metric.FailureCount);
        }

        public void ObserveDuration(string queryName, double milliseconds)
        {
            var metric = _metrics.GetOrAdd(queryName, _ => new Metric());
            Interlocked.Exchange(
                ref Unsafe.As<double, long>(ref metric.TotalDurationMs),
                BitConverter.DoubleToInt64Bits(metric.TotalDurationMs + milliseconds)
            );
        }

        public QueryMetricSnapshot GetSnapshot(string queryName)
        {
            if (!_metrics.TryGetValue(queryName, out var metric))
            {
                return QueryMetricSnapshot.Empty(queryName);
            }

            return new QueryMetricSnapshot(
                queryName,
                metric.TotalCount,
                metric.FailureCount,
                metric.TotalDurationMs
            );
        }
    }
}

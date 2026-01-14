using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Querying;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Renligou.Core.Application.Bus
{
    public class LoggingQueryBus(
        IQueryBus _inner,
        ILogger <LoggingQueryBus> _logger
    ) : IQueryBus
    {
        public async Task<TResult> QueryAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            var queryName = typeof(TQuery).Name;
            var sw = Stopwatch.StartNew();

            _logger.LogInformation(
                "Executing query {QueryName} with payload {@Query}",
                queryName,
                query);

            try
            {
                var result = await _inner.QueryAsync<TQuery, TResult>(query, cancellationToken);

                sw.Stop();
                _logger.LogInformation(
                    "Query {QueryName} executed successfully in {ElapsedMilliseconds} ms",
                    queryName,
                    sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(
                    ex,
                    "Query {QueryName} failed after {ElapsedMilliseconds} ms",
                    queryName,
                    sw.ElapsedMilliseconds);

                throw; // ⚠️ 不吞异常
            }
        }
    }
}

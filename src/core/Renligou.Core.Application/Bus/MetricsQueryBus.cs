using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Querying;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace Renligou.Core.Application.Bus
{
    public class MetricsQueryBus(
        IQueryBus _inner,
        IQueryMetrics _metrics,
        IHostEnvironment _env
    ) : IQueryBus
    {
        public async Task<TResult> QueryAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            var queryName = typeof(TQuery).Name;
            var sw = Stopwatch.StartNew();
            var success = false;

            _metrics.Increment(queryName);

            try
            {
                var result = await _inner.QueryAsync<TQuery, TResult>(query, cancellationToken);
                success = true;
                return result;
            }
            catch
            {
                _metrics.IncrementFailure(queryName);
                throw;
            }
            finally
            {
                sw.Stop();
                _metrics.ObserveDuration(queryName, sw.Elapsed.TotalMilliseconds);

                // ⭐ 只在测试 / 开发环境输出
                if (_env.IsDevelopment() || _env.IsEnvironment("Test"))
                {
                    PrintToConsole<TQuery>(
                        sw.Elapsed.TotalMilliseconds,
                        success
                    );
                }
            }
        }

        private static void PrintToConsole<TQuery>(double elapsedMs, bool success)
        {
            var name = typeof(TQuery).Name;
            var status = success ? "SUCCESS" : "FAIL";

            Console.ForegroundColor = success
                ? ConsoleColor.Green
                : ConsoleColor.Red;

            Console.WriteLine(
                $"[QueryMetrics] {name,-40} | {status,-7} | {elapsedMs,8:0.00} ms");

            Console.ResetColor();
        }
    }
}

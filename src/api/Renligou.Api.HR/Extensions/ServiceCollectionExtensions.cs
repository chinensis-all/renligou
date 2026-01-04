using Microsoft.EntityFrameworkCore;
using Renligou.Core.Infrastructure.Persistence.EFCore;

namespace Renligou.Api.HR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMysql(this IServiceCollection services, string mysqlConnStr, bool isDevelepment, string environmentName)
        {
            services.AddDbContext<MysqlDbContext>(options =>
            {
                options.UseMySql(
                    mysqlConnStr,
                    ServerVersion.AutoDetect(mysqlConnStr)
                ).UseSnakeCaseNamingConvention();


                if (isDevelepment || environmentName == "Testing")
                {
                    options
                        .EnableSensitiveDataLogging()     // 输出参数值
                        .EnableDetailedErrors()           // 输出详细错误
                        .LogTo(Console.WriteLine,         // 打印到控制台
                            new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted, Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandError },
                            LogLevel.Information
                        );
                }
            });

            return services;
        }
    }
}

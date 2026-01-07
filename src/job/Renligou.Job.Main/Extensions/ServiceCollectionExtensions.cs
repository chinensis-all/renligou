using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus;
using Renligou.Core.Infrastructure.Persistence.EFCore;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Shared.Querying;
using Renligou.Job.Main.Kernel;
using Scrutor;
using System.Reflection;

namespace Renligou.Job.Main.Extensions
{
    /// <summary>
    /// Provides extension methods for registering MySQL database contexts with an IServiceCollection.
    /// </summary>
    /// <remarks>This class contains methods that simplify the integration of MySQL Entity Framework Core
    /// contexts into ASP.NET Core dependency injection containers. The extension methods are intended to be used during
    /// application startup to configure database connectivity and related options.</remarks>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMysql(this IServiceCollection services, string mysqlConnStr, bool isDevelepment, string environmentName)
        {
            if (string.IsNullOrEmpty(mysqlConnStr))
            {
                throw new ArgumentException("MySQL connection string is missing.", nameof(mysqlConnStr));
            }

            services.AddDbContext<MysqlDbContext>(options =>
            {
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

                options.UseMySql(mysqlConnStr, serverVersion)
                       .UseSnakeCaseNamingConvention();

                if (isDevelepment || environmentName == "Testing")
                {
                    options
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                        .LogTo(Console.WriteLine,
                            new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted, Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandError },
                            LogLevel.Information
                        );
                }
            });

            services.AddScoped<DbContext>(provider => provider.GetRequiredService<MysqlDbContext>());
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }

        /// <summary>
        /// Registers command and query bus services along with their respective handlers from the specified assemblies.
        /// </summary>
        public static IServiceCollection AddBus(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddScoped<ICommandBus, CommandBus>();
            services.AddScoped<IQueryBus, QueryBus>();

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithScopedLifetime()
            );

            return services;
        }

        /// <summary>
        /// Registers repository services from the specified assemblies.
        /// </summary>
        public static IServiceCollection AddRepository(this IServiceCollection services, Assembly[] assemblies)
        {
            services.Scan(scan => scan
               .FromAssemblies(assemblies) // 统一使用传入的程序集
               .AddClasses(c => c.AssignableTo(typeof(IRepository)))
               .AsImplementedInterfaces()
               .AsSelf()
               .WithScopedLifetime()
            );

            return services;
        }

        /// <summary>
        /// Registers all non-abstract, non-interface classes ending with "Facade" or "AppService" from the specified assemblies.
        /// </summary>
        public static IServiceCollection AddAppFacade(this IServiceCollection services, Assembly[] assemblies)
        {
            services.Scan(scan => scan
               .FromAssemblies(assemblies) // 统一使用传入的程序集
               .AddClasses(c => c.Where(t => t.Name.EndsWith("Facade") && !t.IsAbstract && !t.IsInterface))
               .AsImplementedInterfaces()
               .AsSelf()
               .WithScopedLifetime()
               .AddClasses(c => c.Where(t => t.Name.EndsWith("AppService") && !t.IsAbstract && !t.IsInterface))
               .AsImplementedInterfaces()
               .AsSelf()
               .WithScopedLifetime()
            );

            return services;
        }

        /// <summary>
        /// Adds the outbox worker as a hosted service to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to which the outbox worker will be added. Cannot be null.</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/> that was provided, to support method chaining.</returns>
        public static IServiceCollection AddWorkers(this IServiceCollection services)
        {
            services.AddHostedService<OutboxWorker>();

            return services;
        }
    }
}

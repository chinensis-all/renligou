using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Renligou.Core.Application.Bus;
using Renligou.Core.Infrastructure.Cache;
using Renligou.Core.Infrastructure.External.Metrics;
using Renligou.Core.Infrastructure.Persistence.EFCore;
using Renligou.Core.Infrastructure.Shared.Id;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Cache;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Shared.Querying;
using Scrutor; 
using System.Reflection;
using StackExchange.Redis;

namespace Renligou.Api.Boss.Extensions
{
    /// <summary>
    /// Provides extension methods for registering MySQL database contexts with an IServiceCollection.
    /// </summary>
    /// <remarks>This class contains methods that simplify the integration of MySQL Entity Framework Core
    /// contexts into ASP.NET Core dependency injection containers. The extension methods are intended to be used during
    /// application startup to configure database connectivity and related options.</remarks>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddIdGenerator(configuration);

            services.AddSwagger();

            services.AddMysql(configuration, environment);

            services.AddRepository(new[]
            {
                typeof(Renligou.Core.Infrastructure.InfrastructureLayer).Assembly
            });

            services.AddBus(new[]
            {
                typeof(Renligou.Core.Application.ApplicationLayer).Assembly
            });

            services.AddAppFacade(new[]
            {
                typeof(Renligou.Core.Application.ApplicationLayer).Assembly
            });

            services.AddCache(configuration);

            return services;
        }

        public static IServiceCollection AddIdGenerator(this IServiceCollection services, IConfiguration configuration)
        {
            var workerId = configuration.GetValue<long>("WorkerId");
            services.AddSingleton<IIdGenerator>(new SnowflakeId(workerId));
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            // 使用不同的名称以避免与 Swashbuckle 的 "v1" 冲突
            services.AddOpenApi("openapi", options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info.Title = "Renligou Boss API";
                    document.Info.Version = "v1";
                    document.Info.Description = "Boss接口文档";
                    return Task.CompletedTask;
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo { Title = "Renligou Boss API", Version = "v1" });
                
                var assembly = typeof(ServiceCollectionExtensions).Assembly;
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            });

            return services;
        }

        public static IServiceCollection AddMysql(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            string mysqlConnStr = configuration.GetConnectionString("Mysql")!;
            if (string.IsNullOrEmpty(mysqlConnStr))
            {
                throw new ArgumentException("MySQL connection string is missing.", nameof(mysqlConnStr));
            }

            services.AddDbContext<MysqlDbContext>(options =>
            {
                // 停止使用 AutoDetect，因为它在启动时需要活动的数据库连接，
                // 如果数据库暂时不可用，会导致应用程序在构建 ServiceProvider 时崩溃。
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

                options.UseMySql(mysqlConnStr, serverVersion)
                       .UseSnakeCaseNamingConvention();

                if (environment.IsDevelopment() || environment.IsEnvironment("Testing"))
                {
                    options
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                        .LogTo(Console.WriteLine,
                            new[] { RelationalEventId.CommandExecuted, RelationalEventId.CommandError },
                            LogLevel.Information
                        );
                }
            });

            services.AddScoped<Microsoft.EntityFrameworkCore.DbContext>(provider => provider.GetRequiredService<MysqlDbContext>());
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }

        /// <summary>
        /// Registers command and query bus services along with their respective handlers from the specified assemblies.
        /// </summary>
        public static IServiceCollection AddBus(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton<IQueryMetrics, InMemoryQueryMetrics>();

            services.AddScoped<ICommandBus, CommandBus>();
            services.AddScoped<IQueryBus, QueryBus>();
            services.Decorate<IQueryBus, LoggingQueryBus>();
            services.Decorate<IQueryBus, MetricsQueryBus>();
            services.Decorate<IQueryBus, CachingQueryBus>();

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
        /// Adds in-memory, Redis, and no-op cache implementations to the service collection and configures the default
        /// cache provider.
        /// </summary>
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var providerName = configuration["Cache:Provider"] ?? CacheKeys.Memory;
            var redisConnStr = configuration.GetConnectionString("Redis");

            // 内存
            services.AddMemoryCache();
            services.AddKeyedSingleton<ICache, MemoryCacheAdapter>(CacheKeys.Memory);

            // Redis
            if (!string.IsNullOrEmpty(redisConnStr))
            {
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnStr));
                services.AddKeyedSingleton<ICache, RedisCacheAdapter>(CacheKeys.Redis);
            }

            // 无缓存
            services.AddKeyedSingleton<ICache, NoopCacheAdapter>(CacheKeys.None);

            services.AddSingleton<ICache>(sp =>
            { 
                return sp.GetRequiredKeyedService<ICache>(providerName);
            });

            return services;
        }
    }
}

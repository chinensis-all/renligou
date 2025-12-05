/**
 * Copyright (C) 2025 zhangxihai<mail@sniu.com>，All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 * WARNING: This code is licensed under the GPL. Any derivative work or
 * distribution of this code must also be licensed under the GPL. Failure
 * to comply with the terms of the GPL may result in legal action.
 */
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Renligou.Application.Dynamic;
using Renligou.Contracts;
using Renligou.Contracts.Dynamic;
using Renligou.Contracts.EFCore;
using Renligou.Infras.IdGenerator;
using Renligou.Infras.Persistence.Crud;
using Renligou.Infras.Persistence.EFcore;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Renligou.Boss.Extensions
{
    /// <summary>
    /// ServiceCollectionExtensions: 服务集合扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AddSnowflake: 将SnowflakeId注册为IIdGenerator单例服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflake(this IServiceCollection services)
        {
            services.AddSingleton<IIdGenerator>(new SnowflakeId(workerId: 1));
            return services;
        }

        /// <summary>
        /// AddDynamicCrud: 注册动态CRUD相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicCrud(this IServiceCollection services)
        {
            services.AddSingleton<DynamicCrudRegistry>();
            services.AddScoped<IDynamicCrudRepository, EfDynamicCrudRepository>();
            services.AddScoped<DynamicCrudService>();

            return services;
        }

        /// <summary>
        /// AddCache: 添加缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConnStr"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services, string redisConnStr)
        {
            services.AddFusionCache()
                .WithSerializer(
                    new FusionCacheSystemTextJsonSerializer(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    )
                )
                .WithDistributedCache(
                    new RedisCache(new RedisCacheOptions { Configuration = redisConnStr })
                );
            return services;
        }

        /// <summary>
        /// AddEventCap: 添加 DotNetCore.CAP服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventCap(this IServiceCollection services, RabbitMQOptions options, string mysqlConnStr)
        {
            services.AddCap(x =>
            {
                x.UseRabbitMQ(cfg =>
                {
                    cfg.HostName = options.Host;
                    cfg.Port = options.Port;
                    cfg.UserName = options.UserName;
                    cfg.Password = options.Password;
                });
                x.UseMySql(opt => opt.ConnectionString = mysqlConnStr);
                x.FailedRetryCount = 5;
                x.FailedRetryInterval = 60;
            });
            return services;
        }

        /// <summary>
        /// AddMysql: 添加MySQL数据库上下文
        /// </summary>
        /// <param name="services"></param>
        /// <param name="mysqlConnStr"></param>
        /// <returns></returns>
        public static IServiceCollection AddMysql(this IServiceCollection services, string mysqlConnStr)
        {
            services.AddDbContext<MySQLDBContext>(options =>
            {
                options.UseMySql(
                    mysqlConnStr,
                    ServerVersion.AutoDetect(mysqlConnStr)
                ).UseSnakeCaseNamingConvention();
            });

            return services;
        }
    }
}

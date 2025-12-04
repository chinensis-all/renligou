
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
using Renligou.Application.Dynamic;
using Renligou.Contracts;
using Renligou.Contracts.Dynamic;
using Renligou.Contracts.EFCore;
using Renligou.Infras.Cache;
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
    }
}

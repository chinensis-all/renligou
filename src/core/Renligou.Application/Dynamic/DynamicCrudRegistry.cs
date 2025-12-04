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
using Renligou.Contracts.Dynamic;
using System.Collections.Concurrent;

namespace Renligou.Application.Dynamic
{
    /// <summary>
    /// DynamicCrudRegistry: 动态CRUD注册表
    /// </summary>
    public class DynamicCrudRegistry
    {
        private readonly ConcurrentDictionary<Type, object> _dict = new();

        public void Add<TDto>(EntityConfig<TDto> cfg)
        {
            if (cfg.EntityType == null || cfg.EntityType == default)
            {
                throw new ArgumentException(string.Format("{}的动态查询配置EntityType未配置", cfg.Title));
            }

            if (cfg.DtoType == null)
            {
                throw new ArgumentException(string.Format("{}的动态查询配置DtoType未配置", cfg.Title));
            }

            _dict[cfg.DtoType] = cfg;
        }

        public EntityConfig<TDto> Get<TDto>()
        {
            Type type = typeof(TDto);
            try
            {
                if (_dict.TryGetValue(type, out var c)) return (EntityConfig<TDto>)c;
                throw new Exception($"未知动态查询配置: {type}");
            }
            catch
            {
                throw new Exception($"获取动态查询配置错误: {type}");
            }
        }
    }
}

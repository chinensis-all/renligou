﻿/**
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
using Microsoft.Extensions.Logging;
using Renligou.Contracts;
using Renligou.Contracts.Dynamic;
using Renligou.Contracts.Exceptions;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using ZiggyCreatures.Caching.Fusion;

namespace Renligou.Application.Dynamic
{
    /// <summary>
    /// DynamicCrudService: 动态CRUD服务
    /// </summary>
    public class DynamicCrudService
    {
        private readonly IFusionCache _cache;

        private readonly IDynamicCrudRepository _repo;

        private readonly DynamicCrudRegistry _registry;

        private readonly ILogger _logger;

        public DynamicCrudService(
            IFusionCache cache,
            IDynamicCrudRepository repo,
            DynamicCrudRegistry registry,
            ILogger<DynamicCrudService> logger
        )
        {
            _cache = cache;
            _repo = repo;
            _registry = registry;
            _logger = logger;
        }

        /// <summary>
        /// 创建(数据方式, 推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InternalServerException"></exception>
        public async Task<TDto> CreateAsync<TDto>(IDictionary<string, object?> request)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();
            var e = Activator.CreateInstance(cfg.EntityType)!;

            if (cfg.RequestToEntityMapper == null || cfg.RequestToEntityMapper == default)    // 谨慎反射拷贝，性能杀手          
                Copy(request, e);
            else
                cfg.RequestToEntityMapper.Map(request, e);

            e = await _repo.AddAsync(e);

            if (e == null)
                throw new InternalServerException(string.Format("创建{0}失败", cfg.Title));

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
                return RefConvert<object, TDto>(e);

            TDto dto = Activator.CreateInstance<TDto>();
            if (dto == null)
                throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));

            if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
            {
                CopyObject(e, dto);
                return dto;
            }

            cfg.EntityToDtoMapper.Map(e, dto);
            return dto;
        }

        /// <summary>
        /// 创建(持久化对象方式, 不推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InternalServerException"></exception>
        public async Task<TDto> CreateAsync<TDto, TEntity>(TEntity entity)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            if (cfg.EntityType != typeof(TEntity))
            {
                throw new ArgumentException(string.Format("无法创建{0}，因为实体类型不匹配", cfg.Title));
            }

            if (entity == null)
            {
                throw new ArgumentException(string.Format("无法创建{0}，因为未赋值", cfg.Title));
            }

            var e = await _repo.AddAsync(entity);

            if (e == null)
                throw new InternalServerException(string.Format("创建{0}失败", cfg.Title));

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
                return RefConvert<object, TDto>(e);

            TDto dto = Activator.CreateInstance<TDto>();
            if (dto == null)
                throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));

            if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
            {
                CopyObject(e, dto);
                return dto;
            }

            cfg.EntityToDtoMapper.Map(e, dto);
            return dto;
        }


        /// <summary>
        /// 更新(数据方式, 推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="key"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        /// <exception cref="InternalServerException"></exception>
        public async Task<TDto> ModifyAsync<TDto>(object key, IDictionary<string, object?> request)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            var entity = await _repo.FindAsync(cfg.EntityType, key);
            if (entity == null)
            {
                throw new BadRequestException(string.Format("无法更新{0}，因为未找到{1}:{2}", cfg.Title, cfg.KeyName, key));
            }

            var e = await _repo.UpdateAsync(entity);

            if (e == null)
                throw new InternalServerException(string.Format("更新{0}失败", cfg.Title));

            if (cfg.EnableDetailCache)
            {
                // 清除Detail缓存
                _ = _cache.ExpireAsync(cfg.GetDetailCacheKey(key));
            }

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
                return RefConvert<object, TDto>(e);

            TDto dto = Activator.CreateInstance<TDto>();
            if (dto == null)
                throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));

            if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
            {
                CopyObject(e, dto);
                return dto;
            }

            cfg.EntityToDtoMapper.Map(e, dto);
            return dto;
        }

        /// <summary>
        /// 更新(持久化对象方式, 不推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="BadRequestException"></exception>
        /// <exception cref="InternalServerException"></exception>
        public async Task<TDto> ModifyAsync<TDto, TEntity>(object key, TEntity entity)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            var e = await _repo.UpdateAsync(entity);
            if (entity == null)
                throw new ArgumentException("无法更新实体，因为未赋值");

            if (e == null)
                throw new InternalServerException(string.Format("更新{0}失败", cfg.Title));

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
                return RefConvert<object, TDto>(e);

            if (cfg.EnableDetailCache)
            {
                // 清除Detail缓存
                _ = _cache.ExpireAsync(cfg.GetDetailCacheKey(key));
            }

            TDto dto = Activator.CreateInstance<TDto>();
            if (dto == null)
                throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));

            if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
            {
                CopyObject(e, dto);
                return dto;
            }

            cfg.EntityToDtoMapper.Map(e, dto);
            return dto;
        }

        /// <summary>
        /// 删除(主键模式, 推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>

        public async Task<bool> DestroyAsync<TDto>(object key)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            bool res = await _repo.DeleteAsync(cfg.EntityType, cfg.KeyPropertyInfo, key, cfg.EnableSoftDelete);
            if (res && cfg.EnableDetailCache)
            {
                // 清除Detail缓存
                _ = _cache.ExpireAsync(cfg.GetDetailCacheKey(key));
            }

            return res;
        }

        /// <summary>
        /// 删除(持久化对象模式, 不推荐)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>

        public async Task<bool> DestroyAsync<TDto, TEntity>(TEntity entity)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();
            var keyPropertyInfo = cfg.KeyPropertyInfo;
            var key = keyPropertyInfo.GetValue(entity)!;

            bool res = await _repo.DeleteAsync(cfg.EntityType, keyPropertyInfo, key, cfg.EnableSoftDelete);
            if (res && cfg.EnableDetailCache)
            {
                // 清除Detail缓存
                string cacheKey = cfg.GetDetailCacheKey(key);
                _ = _cache.ExpireAsync(cfg.GetDetailCacheKey(key));
            }

            return res;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<TDto?> GetDetailAsync<TDto>(object key)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            if (cfg.EnableDetailCache)
            {
                _ = _cache.ExpireAsync(cfg.GetDetailCacheKey(key));
            }

            return await GetDetailAsync<TDto>(cfg, key);
        }

        /// <summary>
        /// 获取详情(不带缓存)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="cfg"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="InternalServerException"></exception>
        private async Task<TDto?> GetDetailAsync<TDto>(EntityConfig<TDto> cfg, object key)
        {
            var entity = await _repo.FindAsync(cfg.EntityType, key);

            if (entity == null)
                return default;

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
                return RefConvert<object, TDto>(entity);

            TDto dto = Activator.CreateInstance<TDto>();
            if (dto == null)
                throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));

            if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
            {
                CopyObject(entity, dto);
                return dto;
            }

            cfg.EntityToDtoMapper.Map(entity, dto);
            return dto;
        }

        /// <summary>
        /// 获取键值对列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="criteria"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OptionResult>> GetOptionList<TDto>(Dictionary<string, object?> criteria, long limit)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            if (cfg.EnableOptionCache)
            {
                var list = await _cache.GetOrSetAsync<IEnumerable<OptionResult>>(
                    cfg.GetOptionCacheKey(criteria),
                    async (ct) =>
                    {
                        return await GetOptionList<TDto>(cfg, criteria, limit);
                    },
                    options =>
                    {
                        options.SetDuration(TimeSpan.FromSeconds(cfg.OptionCacheDurationInSeconds));
                    }
                );

                return list ?? Enumerable.Empty<OptionResult>();
            }

            // 未开启缓存，直接查库
            return await GetOptionList<TDto>(cfg, criteria, limit);
        }

        /// <summary>
        /// 获取键值对列表(不带缓存)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="cfg"></param>
        /// <param name="criteria"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private async Task<IEnumerable<OptionResult>> GetOptionList<TDto>(EntityConfig<TDto> cfg, Dictionary<string, object?> criteria, long limit)
        {
            IEnumerable<object> optionList = await _repo.Search(cfg.EntityType, criteria, limit);
            List<OptionResult> results = new List<OptionResult>();

            foreach (var option in optionList)
            {
                results.Add(new OptionResult(
                    code: cfg.KeyPropertyInfo != null && cfg.KeyPropertyInfo != default ? cfg.KeyPropertyInfo.GetValue(option)?.ToString() ?? "" : "",
                    name: cfg.KeyPropertyInfo != null && cfg.KeyPropertyInfo != default ? cfg.KeyPropertyInfo.GetValue(option)?.ToString() ?? "" : ""
                ));
            }

            return results;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InternalServerException"></exception>
        public async Task<PageResult<TDto>> GetPaginationAsync<TDto>(PageRequest request)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            if (cfg.EnablePageCache)
            {
                var pageResult = await _cache.GetOrSetAsync<PageResult<TDto>>(
                    cfg.GetPageCacheKey(request.Page, request.PageSize, request.Criteria),
                    async (ct) =>
                    {
                        return await GetPaginationAsync<TDto>(cfg, request);
                    },
                    options =>
                    {
                        options.SetDuration(TimeSpan.FromSeconds(cfg.PageCacheDurationInSeconds));
                    }
                );

                return pageResult == null ? new PageResult<TDto>(request.Page, request.PageSize, 0, Enumerable.Empty<TDto>()) : pageResult;
            }

            return await GetPaginationAsync<TDto>(cfg, request);
        }

        /// <summary>
        /// 获取分页列表(不带缓存)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="cfg"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InternalServerException"></exception>
        private async Task<PageResult<TDto>> GetPaginationAsync<TDto>(EntityConfig<TDto> cfg, PageRequest request)
        {
            (long total, IEnumerable<object> items) = await _repo.PaginateAsync(
                 cfg.EntityType,
                 request.Criteria,
                 request.Sorts,
                 request.Page,
                 request.PageSize
             );

            List<TDto> dtoItems = new List<TDto>();

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
            {
                dtoItems.AddRange(items.Select(e => RefConvert<object, TDto>(e)));
            }
            else
            {
                dtoItems.AddRange(items.Select(e =>
                {
                    TDto dto = Activator.CreateInstance<TDto>();
                    if (dto == null)
                        throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));
                    if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
                    {
                        CopyObject(e, dto);
                        return dto;
                    }

                    cfg.EntityToDtoMapper.Map(e, dto);
                    return dto;
                }));
            }

            return new PageResult<TDto>(
                page: request.Page,
                pageSize: request.PageSize,
                total: total,
                items: dtoItems
            );
        }

        /// <summary>
        /// 获取偏移分页列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TCursor"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<KesetPageResult<TDto, TCursor>> GetKesetPaginationAsync<TDto, TCursor>(KeysetPageRequest<TCursor> request)
        {
            EntityConfig<TDto> cfg = _registry.Get<TDto>();

            if (cfg.EnablePageCache)
            {
                var pageResult = await _cache.GetOrSetAsync<KesetPageResult<TDto, TCursor>>(
                    cfg.GetKeysetPageCacheKey(request.Limit, request.IsNext, request.Ascending, request.Cursor, request.Criteria),
                    async (ct) =>
                    {
                        return await GetKesetPaginationAsync<TDto, TCursor>(cfg, request);
                    },
                    options =>
                    {
                        options.SetDuration(TimeSpan.FromSeconds(cfg.PageCacheDurationInSeconds));
                    }
                );

                return pageResult == null ? new KesetPageResult<TDto, TCursor>(Array.Empty<TDto>(), default!, default!) : pageResult;
            }

            return await GetKesetPaginationAsync<TDto, TCursor>(cfg, request);
        }


        /// <summary>
        /// 获取偏移分页列表(不带缓存)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TCursor"></typeparam>
        /// <param name="cfg"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InternalServerException"></exception>
        private async Task<KesetPageResult<TDto, TCursor>> GetKesetPaginationAsync<TDto, TCursor>(EntityConfig<TDto> cfg, KeysetPageRequest<TCursor> request)
        {
            (IReadOnlyList<object> Items, object? NextCursor, object? PrevCursor) = await _repo.KesetPaginateAsync(
                 cfg.EntityType,
                 request.Criteria,
                 cfg.KeyPropertyInfo,
                 request.Ascending,
                 request.Limit,
                 request.Cursor!
             );

            List<TDto> dtoItems = new List<TDto>();

            // Dto == Entity
            if (cfg.DtoType == cfg.EntityType)
            {
                dtoItems.AddRange(Items.Select(e => RefConvert<object, TDto>(e)));
            }
            else
            {
                dtoItems.AddRange(Items.Select(e =>
                {
                    TDto dto = Activator.CreateInstance<TDto>();
                    if (dto == null)
                        throw new InternalServerException(string.Format("创建{0}失败，无法实例化DTO", cfg.Title));
                    if (cfg.EntityToDtoMapper == null || cfg.EntityToDtoMapper == default)           // 谨慎反射拷贝，性能杀手
                    {
                        CopyObject(e, dto);
                        return dto;
                    }
                    cfg.EntityToDtoMapper.Map(e, dto);
                    return dto;
                }));
            }

            return new KesetPageResult<TDto, TCursor>(
                items: dtoItems,
                nextCursor: NextCursor != null ? (TCursor)Convert.ChangeType(NextCursor, typeof(TCursor)) : default!,
                prevCursor: PrevCursor != null ? (TCursor)Convert.ChangeType(PrevCursor, typeof(TCursor)) : default!
            );
        }

        /// <summary>
        /// Dictionary赋值给对象(慎用)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void Copy(IDictionary<string, object?> from, object to)
        {
            var toType = to.GetType();

            foreach (var kv in from)
            {
                var p = toType.GetProperty(kv.Key,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (p != null && p.CanWrite)
                {
                    try
                    {
                        var value = kv.Value;

                        if (value == null)
                        {
                            if (!p.PropertyType.IsValueType || Nullable.GetUnderlyingType(p.PropertyType) != null)
                            {
                                p.SetValue(to, null);
                            }
                            continue;
                        }

                        var targetType = p.PropertyType;

                        var underlyingType = Nullable.GetUnderlyingType(targetType);
                        if (underlyingType != null)
                        {
                            targetType = underlyingType;
                        }

                        if (!targetType.IsInstanceOfType(value))
                        {
                            if (targetType == typeof(Guid) && value is string strGuid)
                            {
                                value = Guid.Parse(strGuid);
                            }
                            else if (targetType.IsEnum)
                            {
                                value = Enum.Parse(targetType, value.ToString());
                            }
                            else
                            {
                                value = Convert.ChangeType(value, targetType);
                            }
                        }

                        p.SetValue(to, value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"属性 {p.Name} 赋值失败: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 对象对拷(慎用)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void CopyObject(object source, object destination)
        {
            if (source == null || destination == null) return;

            var sourceType = source.GetType();
            var destType = destination.GetType();

            var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProp in sourceProps)
            {
                if (!sourceProp.CanRead) continue;

                var destProp = destType.GetProperty(sourceProp.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (destProp == null || !destProp.CanWrite) continue;

                try
                {
                    var value = sourceProp.GetValue(source);

                    if (value == null)
                    {
                        if (!destProp.PropertyType.IsValueType || Nullable.GetUnderlyingType(destProp.PropertyType) != null)
                        {
                            destProp.SetValue(destination, null);
                        }
                        continue;
                    }

                    var targetType = destProp.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(targetType);
                    if (underlyingType != null)
                    {
                        targetType = underlyingType;
                    }

                    if (!targetType.IsInstanceOfType(value))
                    {
                        if (targetType == typeof(Guid) && value is string strGuid)
                        {
                            value = Guid.Parse(strGuid);
                        }
                        else if (targetType.IsEnum)
                        {
                            value = Enum.Parse(targetType, value.ToString());
                        }
                        else if (targetType == typeof(string) && value is Guid)
                        {
                            value = value.ToString();
                        }
                        else
                        {
                            value = Convert.ChangeType(value, targetType);
                        }
                    }

                    destProp.SetValue(destination, value);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 无损转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        private TDto RefConvert<T, TDto>(T e)
        {
            return Unsafe.As<T, TDto>(ref e);
        }
    }
}

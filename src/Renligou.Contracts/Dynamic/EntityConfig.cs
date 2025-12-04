
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
namespace Renligou.Contracts.Dynamic
{
    /// <summary>
    /// EntityConfig: 实体配置
    /// </summary>
    public class EntityConfig<TDto>
    {
        public string Title { get; set; } = default!;                                          // 实体标题（用作日志或错误信息）

        public Type EntityType { get; set; } = default!;                                       // 实体类型

        public Type DtoType
        {
            get { return typeof(TDto); }
            private set { }
        }

        public IEntityToDtoMapper<TDto> EntityToDtoMapper { get; set; } = default!;            // 实体到DTO映射器

        public IRequestToEntityMapper RequestToEntityMapper { get; set; } = default!;          // 请求体到实体映射器

        public string KeyName { get; set; } = "Id";                                            // 主键属性名

        public PropertyInfo KeyPropertyInfo { get; set; } = default!;                          // 主键属性信息(一次性获取)

        public bool EnableDetailCache { get; set; } = false;                                   // 是否启用DTO缓存

        public int DetailCacheDurationInSeconds { get; set; } = 60;                            // DTO缓存持续时间（秒）

        public bool EnableOptionCache { get; set; } = false;                                   // 是否启用选项缓存

        public int OptionCacheDurationInSeconds { get; set; } = 600;                           // 选项缓存持续时间（秒）

        public bool EnablePageCache { get; set; } = false;                                     // 是否启用分页缓存

        public int PageCacheDurationInSeconds { get; set; } = 60;                              // 分页缓存持续时间（秒）

        public bool EnableKeysetPageCache { get; set; } = false;                               // 是否启用分页缓存

        public int KeysetPageCacheDurationInSeconds { get; set; } = 60;                        // 分页缓存持续时间（秒）

        public bool EnableSoftDelete { get; set; } = false;                                    // 是否软删除

        public bool EnableQueryOption { get; set; } = false;                                   // 是否启用查询选项

        public PropertyInfo OptionPropertyInfo { get; set; } = default!;                       // 查询选项属性名


        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetDetailCacheKey(object id)
        {
            return $"EntityDetail:{EntityType.FullName}:Id={id}";
        }

        /// <summary>
        /// 获取选项缓存键
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetOptionCacheKey(Dictionary<string, object?> criteria)
        {
            var criteriaString = string.Join("&", criteria.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            var criteriaMd5 = ToMd5(criteriaString);
            return $"EntityOption:{EntityType.FullName}:CriteriaMd5={criteriaMd5}";
        }

        /// <summary>
        /// 获取分页缓存键
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetPageCacheKey(int page, int pageSize, Dictionary<string, object?> criteria)
        {
            var criteriaString = string.Join("&", criteria.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            var criteriaMd5 = ToMd5(criteriaString);
            return $"EntityPage:{EntityType.FullName}:PageNumber={page}:PageSize={pageSize}:CriteriaMd5={criteriaMd5}";
        }

        /// <summary>
        /// 获取主键偏移分页缓存键
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="isNext"></param>
        /// <param name="ascending"></param>
        /// <param name="cursor"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetKeysetPageCacheKey(int limit, bool isNext, bool ascending, object? cursor, Dictionary<string, object?> criteria)
        {
            var criteriaString = string.Join("&", criteria.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            var criteriaMd5 = ToMd5(criteriaString);
            var cursorString = cursor != null ? cursor.ToString()! : "null";
            var cursorMd5 = ToMd5(cursorString);
            return $"EntityKeysetPage:{EntityType.FullName}:Limit={limit}:IsNext={isNext}:Ascending={ascending}:CursorMd5={cursorMd5}:CriteriaMd5={criteriaMd5}";
        }

        /// <summary>
        /// 转MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToMd5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}

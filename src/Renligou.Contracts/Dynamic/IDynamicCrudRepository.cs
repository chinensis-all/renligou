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
using System.Reflection;

namespace Renligou.Contracts.Dynamic
{
    /// <summary>
    /// IDynamicCrudRepository: 动态CRUD仓储接口
    /// </summary>
    public interface IDynamicCrudRepository
    {
        /// <summary>
        /// 添加持久化
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object?> AddAsync(object entity);

        /// <summary>
        /// 更新持久化
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object?> UpdateAsync(object entity);


        /// <summary>
        /// 删除持久化
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="keyPropertyInfo"></param>
        /// <param name="key"></param>
        /// <param name="enableSoftDelete"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Type entityType, PropertyInfo keyPropertyInfo, object key, bool enableSoftDelete);


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<object?> FindAsync(Type entityType, object key);

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="EntityType"></param>
        /// <param name="criteria"></param>
        /// <param name="limit"></param>
        /// <returns></returns>        
        Task<IEnumerable<object>> Search(Type EntityType, Dictionary<string, object?> criteria, long limit);
       
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="EntityType"></param>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<(long total, IEnumerable<object>)> PaginateAsync(Type EntityType, Dictionary<string, object?> criteria, Dictionary<string, bool> sorts, int page, int pageSize);

        /// <summary>
        /// 偏移分页
        /// </summary>
        /// <param name="EntityType"></param>
        /// <param name="criteria"></param>
        /// <param name="cursorPropertyInfo"></param>
        /// <param name="ascending"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        Task<(IReadOnlyList<object> Items, object? NextCursor, object? PrevCursor)> KesetPaginateAsync(Type EntityType, Dictionary<string, object?> criteria, PropertyInfo cursorPropertyInfo, bool ascending, int pageSize, object? cursor); 
    }
}

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
using Microsoft.EntityFrameworkCore;
using Renligou.Contracts.Dynamic;
using Renligou.Infras.Persistence.EFcore;
using System.Linq.Expressions;
using System.Reflection;

namespace Renligou.Infras.Persistence.Crud
{
    /// <summary>
    /// EfDynamicCrudRepository: 动态CRUD仓储实现（基于EF Core）
    /// </summary>
    public class EfDynamicCrudRepository : IDynamicCrudRepository
    {
        private readonly MySQLDBContext _context;

        // 缓存 Set<T> 方法的 MethodInfo 以提高反射性能
        private static readonly MethodInfo _setMethod = typeof(DbContext)
            .GetMethods()
            .First(p => p.Name == "Set" && p.IsGenericMethod && p.GetParameters().Length == 0);

        public EfDynamicCrudRepository(MySQLDBContext context)
        {
            _context = context;
        }

        public async Task<object?> AddAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<object?> UpdateAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Type entityType, PropertyInfo keyPropertyInfo, object key, bool enableSoftDelete)
        {
            // 1. 先查找实体
            var entity = await _context.FindAsync(entityType, key);
            if (entity == null) return false;

            if (enableSoftDelete)
            {
                // 2. 软删除：尝试查找名为 IsDeleted 的属性（或你可以约定一个接口）
                var isDeletedProp = entityType.GetProperty("IsDeleted", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
                {
                    isDeletedProp.SetValue(entity, true);
                    _context.Update(entity);
                }
                else
                {
                    // 如果启用了软删除但找不到字段，抛出异常还是执行硬删除取决于业务策略，这里选择抛出提示
                    throw new InvalidOperationException($"Type {entityType.Name} does not have an 'IsDeleted' property for soft deletion.");
                }
            }
            else
            {
                // 3. 硬删除
                _context.Remove(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<object?> FindAsync(Type entityType, object key)
        {
            return await _context.FindAsync(entityType, key);
        }

        public async Task<IEnumerable<object>> Search(Type entityType, Dictionary<string, object?> criteria, long limit)
        {
            var query = GetQueryable(entityType);
            query = ApplyCriteria(query, entityType, criteria);

            // 限制条数
            if (limit > 0)
            {
                // 由于是非泛型 IQueryable，需要使用反射调用 Take，或者转为动态类型
                // 简单方式：使用动态 LINQ 扩展或构建表达式。这里展示构建表达式。
                var callTake = Expression.Call(
                    typeof(Queryable),
                    "Take",
                    new Type[] { entityType },
                    query.Expression,
                    Expression.Constant((int)limit)
                );
                query = query.Provider.CreateQuery(callTake);
            }

            // 执行查询 (Cast to generic list via dynamic usually, or helper)
            return await ToListAsyncDynamic(query, entityType);
        }

        public async Task<(long total, IEnumerable<object>)> PaginateAsync(Type entityType, Dictionary<string, object?> criteria, Dictionary<string, bool> sorts, int page, int pageSize)
        {
            var query = GetQueryable(entityType);
            query = ApplyCriteria(query, entityType, criteria);

            // 1. 获取总数 (CountAsync)
            // 使用 Queryable.Count(query) 的表达式版本
            var countCall = Expression.Call(
                typeof(Queryable),
                "Count",
                new Type[] { entityType },
                query.Expression
            );
            // 执行 Count
            var total = await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                await (Task<int>)InvokeEntityFrameworkExtension("CountAsync", entityType, query));

            // 2. 应用排序
            query = ApplySorts(query, entityType, sorts);

            // 3. 分页 (Skip/Take)
            var skip = (page - 1) * pageSize;
            if (skip < 0) skip = 0;

            var skipCall = Expression.Call(typeof(Queryable), "Skip", new Type[] { entityType }, query.Expression, Expression.Constant(skip));
            query = query.Provider.CreateQuery(skipCall);

            var takeCall = Expression.Call(typeof(Queryable), "Take", new Type[] { entityType }, query.Expression, Expression.Constant(pageSize));
            query = query.Provider.CreateQuery(takeCall);

            // 4. 获取数据
            var list = await ToListAsyncDynamic(query, entityType);

            return (total, list);
        }

        public async Task<(IReadOnlyList<object> Items, object? NextCursor, object? PrevCursor)> KesetPaginateAsync(Type entityType, Dictionary<string, object?> criteria, PropertyInfo cursorPropertyInfo, bool ascending, int pageSize, object? cursor)
        {
            var query = GetQueryable(entityType);
            query = ApplyCriteria(query, entityType, criteria);

            // 1. 应用 Keyset 过滤 (WHERE Cursor > Value)
            if (cursor != null)
            {
                var param = Expression.Parameter(entityType, "x");
                var prop = Expression.Property(param, cursorPropertyInfo);
                var val = Expression.Constant(Convert.ChangeType(cursor, cursorPropertyInfo.PropertyType));

                Expression comparison = ascending
                    ? Expression.GreaterThan(prop, val)
                    : Expression.LessThan(prop, val);

                var lambda = Expression.Lambda(comparison, param);

                var whereCall = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { entityType },
                    query.Expression,
                    lambda
                );
                query = query.Provider.CreateQuery(whereCall);
            }

            // 2. 排序 (必须基于 Cursor 字段排序)
            var sortMethod = ascending ? "OrderBy" : "OrderByDescending";
            var paramSort = Expression.Parameter(entityType, "s");
            var propSort = Expression.Property(paramSort, cursorPropertyInfo);
            var sortLambda = Expression.Lambda(propSort, paramSort);

            var orderCall = Expression.Call(
                typeof(Queryable),
                sortMethod,
                new Type[] { entityType, cursorPropertyInfo.PropertyType },
                query.Expression,
                sortLambda
            );
            query = query.Provider.CreateQuery(orderCall);

            // 3. Take (Fetch pageSize + 1 to check if next page exists is a common strategy, 
            // but here we just take pageSize)
            var takeCall = Expression.Call(typeof(Queryable), "Take", new Type[] { entityType }, query.Expression, Expression.Constant(pageSize));
            query = query.Provider.CreateQuery(takeCall);

            var items = (await ToListAsyncDynamic(query, entityType)).ToList();

            // 4. 计算 NextCursor
            object? nextCursor = null;
            if (items.Count > 0)
            {
                var lastItem = items.Last();
                nextCursor = cursorPropertyInfo.GetValue(lastItem);
            }

            // Note: PrevCursor in keyset pagination is complex without bidirectional queries. 
            // Simply returning null here as strictly backward navigation requires retaining previous state or reverse query.
            return (items, nextCursor, null);
        }

        #region Helper Methods (The Core "Magic")

        /// <summary>
        /// 获取非泛型 IQueryable 并应用 AsNoTracking
        /// </summary>
        private IQueryable GetQueryable(Type entityType)
        {
            // 动态调用 _context.Set<T>()
            var set = _setMethod.MakeGenericMethod(entityType).Invoke(_context, null);
            var query = (IQueryable)set!;

            // 动态调用 .AsNoTracking() 以提升读性能
            // EF Core 扩展方法是静态的，需要在 EntityFrameworkQueryableExtensions 中找
            var asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethod("AsNoTracking")!
                .MakeGenericMethod(entityType);

            return (IQueryable)asNoTrackingMethod.Invoke(null, new object[] { query })!;
        }

        /// <summary>
        /// 动态构建 WHERE 子句
        /// </summary>
        private IQueryable ApplyCriteria(IQueryable query, Type entityType, Dictionary<string, object?> criteria)
        {
            if (criteria == null || criteria.Count == 0) return query;

            var param = Expression.Parameter(entityType, "x");
            Expression? body = null;

            foreach (var pair in criteria)
            {
                // 获取属性表达式 x.Property
                var prop = Expression.Property(param, pair.Key);

                // 处理值常量，必须处理类型转换（例如数据库是 int，传入的是 long）
                object? valObj = pair.Value;
                Expression constant;

                if (valObj != null)
                {
                    // 处理 Nullable 类型不匹配问题
                    var targetType = prop.Type;
                    // 如果是 Nullable<T>，获取底层类型
                    var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                    var safeValue = Convert.ChangeType(valObj, underlyingType);
                    constant = Expression.Constant(safeValue, targetType);
                }
                else
                {
                    constant = Expression.Constant(null, prop.Type);
                }

                var equal = Expression.Equal(prop, constant);

                body = body == null ? equal : Expression.AndAlso(body, equal);
            }

            if (body != null)
            {
                var lambda = Expression.Lambda(body, param);
                var whereCall = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { entityType },
                    query.Expression,
                    lambda
                );
                return query.Provider.CreateQuery(whereCall);
            }

            return query;
        }

        /// <summary>
        /// 动态构建 OrderBy
        /// </summary>
        private IQueryable ApplySorts(IQueryable query, Type entityType, Dictionary<string, bool> sorts)
        {
            if (sorts == null || sorts.Count == 0) return query;

            bool isFirst = true;

            foreach (var sort in sorts)
            {
                var param = Expression.Parameter(entityType, "s");
                var prop = Expression.Property(param, sort.Key);
                var lambda = Expression.Lambda(prop, param);

                string methodName = "";
                if (isFirst)
                    methodName = sort.Value ? "OrderBy" : "OrderByDescending";
                else
                    methodName = sort.Value ? "ThenBy" : "ThenByDescending";

                var orderCall = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { entityType, prop.Type },
                    query.Expression,
                    lambda
                );

                query = query.Provider.CreateQuery(orderCall);
                isFirst = false;
            }

            return query;
        }

        /// <summary>
        /// 动态执行 ToListAsync
        /// </summary>
        private async Task<IEnumerable<object>> ToListAsyncDynamic(IQueryable query, Type entityType)
        {
            // 我们不能直接 await (IQueryable<T>)query，因为我们不知道 T
            // 我们需要反射调用 EntityFrameworkQueryableExtensions.ToListAsync<T>

            var taskObj = InvokeEntityFrameworkExtension("ToListAsync", entityType, query);
            
            // taskObj 是 Task<List<T>>，需要等待它完成
            await (dynamic)taskObj;
            
            // 获取 Task.Result
            var resultProperty = taskObj.GetType().GetProperty("Result");
            var list = resultProperty!.GetValue(taskObj);

            // 转换结果为 IEnumerable<object>
            return ((System.Collections.IEnumerable)list!).Cast<object>();
        }

        /// <summary>
        /// 辅助方法：反射调用 EF Core 的静态扩展方法 (ToListAsync, CountAsync 等)
        /// </summary>
        private object InvokeEntityFrameworkExtension(string methodName, Type entityType, IQueryable query)
        {
            // 查找扩展方法。注意：这里简单查找，实际上可能有多个同名重载（带CancellationToken的）
            // 为了性能，建议将 MethodInfo 缓存到静态字典中
            var method = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == methodName && m.GetParameters().Length == 2) // 假设我们不传 CancellationToken
                .MakeGenericMethod(entityType);

            return method.Invoke(null, new object[] { query, CancellationToken.None })!;
        }

        #endregion
    }
}

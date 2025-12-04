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
using Renligou.Contracts.Exceptions;
using Renligou.Infras.Persistence.Pos;
using Renligou.Application.Test;

namespace Renligou.Infras.Persistence.Mappers
{
    /// <summary>
    /// TestMapper: 示例映射器
    /// </summary>
    public class TestMapper : IRequestToEntityMapper, IEntityToDtoMapper<TestDto>
    {
        public void Map(IDictionary<string, object?> request, object entity)
        {
            if (entity is not TestPo po)
            {
                throw new InternalServerException("请求参数不是 Test 类型，转换失败");
            }

            if (request.TryGetValue("Name", out var nameObj))
            {
                po.Name = Convert.ToString(nameObj) ?? string.Empty;
            }

            if (request.TryGetValue("Category", out var categoryObj))
            {
                po.Category = Convert.ToString(categoryObj) ?? string.Empty;
            }

            if (request.TryGetValue("Price", out var priceObj))
            {
                po.Price = Convert.ToDecimal(priceObj);
            }

            if (request.TryGetValue("Stock", out var stockObj))
            {
                po.Stock = Convert.ToInt32(stockObj);
            }

            if (request.TryGetValue("IsDeleted", out var isDeletedObj))
            {
                po.IsDeleted = Convert.ToBoolean(isDeletedObj);
            }

            if (request.TryGetValue("CreatedAt", out var createdAtObj))
            {
                po.CreatedAt = Convert.ToDateTime(createdAtObj);
            }
        }

        public void Map(object entity, TestDto dto)
        {
            if (entity is not TestPo po)
            {
                throw new InternalServerException("请求参数不是 Test 类型，转换失败");
            }

            dto.Id = po.Id;
            dto.Name = po.Name;
            dto.Category = po.Category;
            dto.Price = po.Price.ToString("F2");
            dto.Stock = po.Stock;
            dto.IsDeleted = po.IsDeleted;
            dto.CreatedAt = po.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

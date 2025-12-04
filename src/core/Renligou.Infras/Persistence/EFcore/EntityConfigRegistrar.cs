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
using Renligou.Contracts.Dynamic;
using Renligou.Application.Test;
using Renligou.Infras.Persistence.Pos;
using Renligou.Infras.Persistence.Mappers;

namespace Renligou.Infras.Persistence.EFcore
{
    /// <summary>
    /// EntityConfigRegistrar: 动态CRUD实体配置注册器
    /// </summary>
    public class EntityConfigRegistrar
    {
        public static void Register(DynamicCrudRegistry registry)
        {
            // 测试用的实体配置
            TestMapper mapper = new TestMapper();
            registry.Add(new EntityConfig<TestDto>
            {
                Title = "测试",
                EntityType = typeof(TestPo),
                EntityToDtoMapper = mapper,
                RequestToEntityMapper = mapper,
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                EnableOptionCache = true,
                EnablePageCache = true,
                EnableKeysetPageCache = true,
                EnableQueryOption = true,
                OptionPropertyInfo = typeof(TestPo).GetProperty("Name")!
            });
        }
    }
}

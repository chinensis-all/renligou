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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Renligou.Application.Dynamic;
using Renligou.Contracts;
using Renligou.Contracts.Dynamic;
using Renligou.Contracts.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZiggyCreatures.Caching.Fusion;

namespace Renligou.Application.Tests.Dynamic
{
    /// <summary>
    /// Contains unit tests for the DynamicCrudService, verifying CRUD operations and mapping logic using test entities
    /// and DTOs.
    /// </summary>
    /// <remarks>These tests validate the behavior of DynamicCrudService when creating entities from
    /// dictionaries and using custom mappers. The class includes test data models and mapping implementations to
    /// simulate real-world scenarios. Use these tests as examples for extending or verifying CRUD functionality in
    /// similar service implementations.</remarks>
    public class DynamicCrudServiceTests
    {
        internal record TestDto
        {
            public int Id { get; set; }

            public string Name { get; set; } = default!;

            public string Category { get; set; } = default!;

            public string Price { get; set; } = default!;

            public int Stock { get; set; }

            public bool IsDeleted { get; set; }

            public String CreatedAt { get; set; } = default!;
        }

        [Table("tests")]
        internal class TestPo
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string Name { get; set; } = string.Empty;

            [MaxLength(50)]
            public string Category { get; set; } = string.Empty;

            [Column(TypeName = "decimal(10, 2)")]
            public decimal Price { get; set; }

            public int Stock { get; set; }

            // 软删除字段
            public bool IsDeleted { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        internal class TestMapper : IRequestToEntityMapper, IEntityToDtoMapper<TestDto>
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

        /// <summary>
        /// CreateAsync_WithDictionary_CopiesValuesAndReturnsDto: 使用字典创建实体，验证值复制和返回的 DTO 是否正确
        /// </summary>
        /// <returns></returns>
        [Fact]
        internal async Task CreateAsync_WithDictionary_CopiesValuesAndReturnsDto()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                EnableOptionCache = true,
                EnablePageCache = true,
                EnableKeysetPageCache = true,
                EnableQueryOption = true,
                OptionPropertyInfo = typeof(TestPo).GetProperty("Name")!,
            };

            registry.Add<TestDto>(cfg);

            repoMock.AddAsync(Arg.Any<object>())
                    .Returns(callInfo => callInfo.Arg<object>());

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            var now = DateTime.UtcNow;
            var request = new Dictionary<string, object?>()
            {
                ["Name"] = "采蘑菇的小姑娘",
                ["Category"] = "小说",
                ["Price"] = 29.99m,
                ["Stock"] = 100,
                ["IsDeleted"] = false,
                ["CreatedAt"] = now
            };

            var dto = await service.CreateAsync<TestDto>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("采蘑菇的小姑娘", dto.Name);
            Assert.Equal("小说", dto.Category);
            Assert.Equal("29.99", dto.Price);
            Assert.Equal(100, dto.Stock);
            Assert.False(dto.IsDeleted);
            //Assert.Equal(now.ToString("yyyy/MM/dd H:m:s"), dto.CreatedAt);
        }

        /// <summary>
        /// CreateAsync_WithDictionary_MapperValuesAndReturnsDto: 使用字典创建实体，使用映射器转换值并返回 DTO
        /// </summary>
        /// <returns></returns>
        [Fact]
        internal async Task CreateAsync_WithDictionary_MapperValuesAndReturnsDto()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var mapper = new TestMapper();
            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                EnableOptionCache = true,
                EnablePageCache = true,
                EnableKeysetPageCache = true,
                EnableQueryOption = true,
                OptionPropertyInfo = typeof(TestPo).GetProperty("Name")!,
                RequestToEntityMapper = mapper,
                EntityToDtoMapper = mapper
            };

            registry.Add<TestDto>(cfg);

            repoMock.AddAsync(Arg.Any<object>())
                    .Returns(callInfo => callInfo.Arg<object>());

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            var now = DateTime.UtcNow;
            var request = new Dictionary<string, object?>()
            {
                ["Name"] = "爱吃鱼的大脸猫",
                ["Category"] = "神话",
                ["Price"] = 199m,
                ["Stock"] = 101,
                ["IsDeleted"] = true,
                ["CreatedAt"] = now
            };

            var dto = await service.CreateAsync<TestDto>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("爱吃鱼的大脸猫", dto.Name);
            Assert.Equal("神话", dto.Category);
            Assert.Equal("199.00", dto.Price);
            Assert.Equal(101, dto.Stock);
            Assert.True(dto.IsDeleted);
            Assert.Equal(now.ToString("yyyy-MM-dd HH:mm:ss"), dto.CreatedAt);
        }

        [Fact]
        internal async Task ModifyAsync_WithValidKey_ShouldUpdateAndReturnDto()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var mapper = new TestMapper();
            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                RequestToEntityMapper = mapper,
                EntityToDtoMapper = mapper
            };

            registry.Add<TestDto>(cfg);

            var existingEntity = new TestPo
            {
                Id = 1,
                Name = "Old Name",
                Category = "Old Category",
                Price = 50m,
                Stock = 10,
                CreatedAt = DateTime.UtcNow
            };

            repoMock.FindAsync(cfg.EntityType, 1).Returns(existingEntity);
            repoMock.UpdateAsync(Arg.Any<object>()).Returns(callInfo => callInfo.Arg<object>());

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            var updateRequest = new Dictionary<string, object?>
            {
                ["Name"] = "Updated Name",
                ["Price"] = 99.99m
            };

            // Act
            var dto = await service.ModifyAsync<TestDto>(1, updateRequest);

            // Assert
            Assert.NotNull(dto);
            await repoMock.Received(1).FindAsync(cfg.EntityType, 1);
            await repoMock.Received(1).UpdateAsync(Arg.Any<object>());
        }

        [Fact]
        internal async Task DestroyAsync_WithValidKey_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                EnableSoftDelete = false
            };

            registry.Add<TestDto>(cfg);

            repoMock.DeleteAsync(
                cfg.EntityType,
                cfg.KeyPropertyInfo,
                1,
                cfg.EnableSoftDelete
            ).Returns(true);

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            // Act
            var result = await service.DestroyAsync<TestDto>(1);

            // Assert
            Assert.True(result);
            await repoMock.Received(1).DeleteAsync(
                cfg.EntityType,
                cfg.KeyPropertyInfo,
                1,
                cfg.EnableSoftDelete
            );
        }

        [Fact]
        internal async Task GetOptionList_ShouldReturnOptionResults()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableOptionCache = false
            };

            registry.Add<TestDto>(cfg);

            var entities = new List<object>
            {
                new TestPo { Id = 1, Name = "Option 1" },
                new TestPo { Id = 2, Name = "Option 2" }
            };

            repoMock.Search(cfg.EntityType, Arg.Any<Dictionary<string, object?>>(), Arg.Any<long>())
                .Returns(entities);

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            // Act
            var result = await service.GetOptionList<TestDto>(new Dictionary<string, object?>(), 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        internal async Task GetPaginationAsync_ShouldReturnPageResult()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var mapper = new TestMapper();
            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnablePageCache = false,
                EntityToDtoMapper = mapper
            };

            registry.Add<TestDto>(cfg);

            var entities = new List<object>
            {
                new TestPo { Id = 1, Name = "Product 1", Category = "Cat1", Price = 10m, Stock = 5, CreatedAt = DateTime.UtcNow },
                new TestPo { Id = 2, Name = "Product 2", Category = "Cat2", Price = 20m, Stock = 10, CreatedAt = DateTime.UtcNow }
            };

            repoMock.PaginateAsync(
                cfg.EntityType,
                Arg.Any<Dictionary<string, object?>>(),
                Arg.Any<Dictionary<string, bool>>(),
                Arg.Any<int>(),
                Arg.Any<int>()
            ).Returns((5L, entities));

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            var pageRequest = new PageRequest
            {
                Page = 1,
                PageSize = 10,
                Criteria = new Dictionary<string, object?>(),
                Sorts = new Dictionary<string, bool>()
            };

            // Act
            var result = await service.GetPaginationAsync<TestDto>(pageRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(5, result.Total);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        internal async Task GetKesetPaginationAsync_ShouldReturnKesetPageResult()
        {
            // Arrange
            var cacheMock = Substitute.For<IFusionCache>();
            var repoMock = Substitute.For<IDynamicCrudRepository>();
            var loggerMock = Substitute.For<ILogger<DynamicCrudService>>();
            var registry = new DynamicCrudRegistry();

            var mapper = new TestMapper();
            var cfg = new EntityConfig<TestDto>
            {
                Title = "TestEntity",
                EntityType = typeof(TestPo),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableKeysetPageCache = false,
                EntityToDtoMapper = mapper
            };

            registry.Add<TestDto>(cfg);

            var entities = new List<object>
            {
                new TestPo { Id = 1, Name = "Product 1", Category = "Cat1", Price = 10m, Stock = 5, CreatedAt = DateTime.UtcNow },
                new TestPo { Id = 2, Name = "Product 2", Category = "Cat2", Price = 20m, Stock = 10, CreatedAt = DateTime.UtcNow }
            };

            repoMock.KesetPaginateAsync(
                cfg.EntityType,
                Arg.Any<Dictionary<string, object?>>(),
                Arg.Any<System.Reflection.PropertyInfo>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<object>()
            ).Returns((entities.AsReadOnly(), 3, 0));

            var service = new DynamicCrudService(cacheMock, repoMock, registry, loggerMock);

            var keysetRequest = new KeysetPageRequest<int>
            {
                Limit = 10,
                IsNext = true,
                Ascending = true,
                Cursor = 0,
                Criteria = new Dictionary<string, object?>()
            };

            // Act
            var result = await service.GetKesetPaginationAsync<TestDto, int>(keysetRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.NextCursor);
            Assert.Equal(0, result.PrevCursor);
        }
    }
}

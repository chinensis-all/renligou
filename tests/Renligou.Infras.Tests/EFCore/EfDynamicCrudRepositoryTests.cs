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

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Renligou.Infras.Persistence.Crud;
using Renligou.Infras.Persistence.EFcore;
using Renligou.Infras.Persistence.Pos;

namespace Renligou.Infras.Tests.EFCore
{
    public class EfDynamicCrudRepositoryTests : IDisposable
    {
        private readonly MySQLDBContext _context;
        private readonly EfDynamicCrudRepository _repository;

        public EfDynamicCrudRepositoryTests()
        {
            // 使用 InMemory 数据库进行测试
            var options = new DbContextOptionsBuilder<MySQLDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MySQLDBContext(options);
            _repository = new EfDynamicCrudRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity_WhenEntityIsValid()
        {
            // Arrange
            var entity = new TestPo
            {
                Name = "Test Product",
                Category = "Electronics",
                Price = 99.99m,
                Stock = 10,
                CreatedAt = DateTime.Now
            };

            // Act
            var result = await _repository.AddAsync(entity);

            // Assert
            result.Should().NotBeNull();
            var addedEntity = result as TestPo;
            addedEntity.Should().NotBeNull();
            addedEntity!.Id.Should().BeGreaterThan(0);
            addedEntity.Name.Should().Be("Test Product");

            // 验证数据库中确实存在
            var dbEntity = await _context.tests.FindAsync(addedEntity.Id);
            dbEntity.Should().NotBeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenEntityIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null!));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new TestPo
            {
                Name = "Original Name",
                Category = "Category1",
                Price = 50m,
                Stock = 5,
                CreatedAt = DateTime.Now
            };
            await _context.tests.AddAsync(entity);
            await _context.SaveChangesAsync();
            _context.Entry(entity).State = EntityState.Detached;

            // Modify
            entity.Name = "Updated Name";
            entity.Price = 75m;

            // Act
            var result = await _repository.UpdateAsync(entity);

            // Assert
            result.Should().NotBeNull();
            var updatedEntity = result as TestPo;
            updatedEntity!.Name.Should().Be("Updated Name");
            updatedEntity.Price.Should().Be(75m);

            // 验证数据库中更新
            var dbEntity = await _context.tests.FindAsync(entity.Id);
            dbEntity!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenEntityIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldHardDeleteEntity_WhenSoftDeleteIsDisabled()
        {
            // Arrange
            var entity = new TestPo
            {
                Name = "To Delete",
                Category = "Test",
                Price = 10m,
                Stock = 1,
                CreatedAt = DateTime.Now
            };
            await _context.tests.AddAsync(entity);
            await _context.SaveChangesAsync();
            var entityId = entity.Id;

            var keyProperty = typeof(TestPo).GetProperty("Id")!;

            // Act
            var result = await _repository.DeleteAsync(typeof(TestPo), keyProperty, entityId, enableSoftDelete: false);

            // Assert
            result.Should().BeTrue();
            var dbEntity = await _context.tests.FindAsync(entityId);
            dbEntity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteEntity_WhenSoftDeleteIsEnabled()
        {
            // Arrange
            var entity = new TestPo
            {
                Name = "To Soft Delete",
                Category = "Test",
                Price = 20m,
                Stock = 2,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            await _context.tests.AddAsync(entity);
            await _context.SaveChangesAsync();
            var entityId = entity.Id;

            var keyProperty = typeof(TestPo).GetProperty("Id")!;

            // Act
            var result = await _repository.DeleteAsync(typeof(TestPo), keyProperty, entityId, enableSoftDelete: true);

            // Assert
            result.Should().BeTrue();
            var dbEntity = await _context.tests.FindAsync(entityId);
            dbEntity.Should().NotBeNull();
            dbEntity!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Arrange
            var keyProperty = typeof(TestPo).GetProperty("Id")!;

            // Act
            var result = await _repository.DeleteAsync(typeof(TestPo), keyProperty, 99999, enableSoftDelete: false);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task FindAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new TestPo
            {
                Name = "Find Me",
                Category = "Test",
                Price = 30m,
                Stock = 3,
                CreatedAt = DateTime.Now
            };
            await _context.tests.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(typeof(TestPo), entity.Id);

            // Assert
            result.Should().NotBeNull();
            var foundEntity = result as TestPo;
            foundEntity!.Name.Should().Be("Find Me");
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _repository.FindAsync(typeof(TestPo), 99999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Search_ShouldReturnMatchingEntities_WhenCriteriaMatch()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>
            {
                { "Category", "Electronics" }
            };

            // Act
            var results = await _repository.Search(typeof(TestPo), criteria, limit: 10);

            // Assert
            results.Should().NotBeEmpty();
            var list = results.Cast<TestPo>().ToList();
            list.Should().HaveCount(2);
            list.Should().OnlyContain(p => p.Category == "Electronics");
        }

        [Fact]
        public async Task Search_ShouldReturnLimitedResults_WhenLimitIsSpecified()
        {
            // Arrange
            await SeedTestData();

            // Act
            var results = await _repository.Search(typeof(TestPo), new Dictionary<string, object?>(), limit: 2);

            // Assert
            results.Should().HaveCount(2);
        }

        [Fact]
        public async Task Search_ShouldReturnAllResults_WhenNoCriteria()
        {
            // Arrange
            await SeedTestData();

            // Act
            var results = await _repository.Search(typeof(TestPo), new Dictionary<string, object?>(), limit: 0);

            // Assert
            results.Should().HaveCount(5);
        }

        [Fact]
        public async Task PaginateAsync_ShouldReturnCorrectPageAndTotal()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>();
            var sorts = new Dictionary<string, bool> { { "Price", true } }; // 按 Price 升序

            // Act
            var (total, items) = await _repository.PaginateAsync(typeof(TestPo), criteria, sorts, page: 1, pageSize: 2);

            // Assert
            total.Should().Be(5);
            items.Should().HaveCount(2);
            var list = items.Cast<TestPo>().ToList();
            list[0].Price.Should().BeLessThan(list[1].Price); // 验证排序
        }

        [Fact]
        public async Task PaginateAsync_ShouldReturnSecondPage_WhenPageIs2()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>();
            var sorts = new Dictionary<string, bool> { { "Id", true } };

            // Act
            var (total, items) = await _repository.PaginateAsync(typeof(TestPo), criteria, sorts, page: 2, pageSize: 2);

            // Assert
            total.Should().Be(5);
            items.Should().HaveCount(2);
        }

        [Fact]
        public async Task PaginateAsync_ShouldApplyCriteria_WhenCriteriaProvided()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>
            {
                { "Category", "Books" }
            };
            var sorts = new Dictionary<string, bool> { { "Price", true } };

            // Act
            var (total, items) = await _repository.PaginateAsync(typeof(TestPo), criteria, sorts, page: 1, pageSize: 10);

            // Assert
            total.Should().Be(2);
            items.Should().HaveCount(2);
            items.Cast<TestPo>().Should().OnlyContain(p => p.Category == "Books");
        }

        [Fact]
        public async Task KesetPaginateAsync_ShouldReturnFirstPage_WhenCursorIsNull()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>();
            var cursorProperty = typeof(TestPo).GetProperty("Id")!;

            // Act
            var (items, nextCursor, prevCursor) = await _repository.KesetPaginateAsync(
                typeof(TestPo), criteria, cursorProperty, ascending: true, pageSize: 2, cursor: null);

            // Assert
            items.Should().HaveCount(2);
            nextCursor.Should().NotBeNull();
            prevCursor.Should().BeNull(); // 第一页没有 PrevCursor
        }

        [Fact]
        public async Task KesetPaginateAsync_ShouldReturnNextPage_WhenCursorProvided()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>();
            var cursorProperty = typeof(TestPo).GetProperty("Id")!;

            // 第一次查询
            var (firstPageItems, firstNextCursor, _) = await _repository.KesetPaginateAsync(
                typeof(TestPo), criteria, cursorProperty, ascending: true, pageSize: 2, cursor: null);

            // Act - 使用第一页的 nextCursor 查询第二页
            var (secondPageItems, secondNextCursor, _) = await _repository.KesetPaginateAsync(
                typeof(TestPo), criteria, cursorProperty, ascending: true, pageSize: 2, cursor: firstNextCursor);

            // Assert
            secondPageItems.Should().HaveCount(2);
            var firstIds = firstPageItems.Cast<TestPo>().Select(p => p.Id).ToList();
            var secondIds = secondPageItems.Cast<TestPo>().Select(p => p.Id).ToList();
            firstIds.Should().NotIntersectWith(secondIds); // 两页数据不应重叠
        }

        [Fact]
        public async Task KesetPaginateAsync_ShouldReturnDescendingOrder_WhenAscendingIsFalse()
        {
            // Arrange
            await SeedTestData();

            var criteria = new Dictionary<string, object?>();
            var cursorProperty = typeof(TestPo).GetProperty("Id")!;

            // Act
            var (items, _, _) = await _repository.KesetPaginateAsync(
                typeof(TestPo), criteria, cursorProperty, ascending: false, pageSize: 3, cursor: null);

            // Assert
            items.Should().HaveCount(3);
            var list = items.Cast<TestPo>().ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i].Id.Should().BeGreaterThan(list[i + 1].Id); // 验证降序
            }
        }

        // 辅助方法：生成测试数据
        private async Task SeedTestData()
        {
            var testData = new List<TestPo>
            {
                new TestPo { Name = "Product 1", Category = "Electronics", Price = 100m, Stock = 10, CreatedAt = DateTime.Now },
                new TestPo { Name = "Product 2", Category = "Electronics", Price = 200m, Stock = 20, CreatedAt = DateTime.Now },
                new TestPo { Name = "Product 3", Category = "Books", Price = 50m, Stock = 30, CreatedAt = DateTime.Now },
                new TestPo { Name = "Product 4", Category = "Books", Price = 75m, Stock = 15, CreatedAt = DateTime.Now },
                new TestPo { Name = "Product 5", Category = "Clothing", Price = 150m, Stock = 25, CreatedAt = DateTime.Now }
            };

            await _context.tests.AddRangeAsync(testData);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }
    }
}

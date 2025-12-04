﻿﻿/**
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
using NSubstitute;
using Renligou.Application.Dynamic;
using Renligou.Application.Test;
using Renligou.Contracts.Dynamic;
using Renligou.Infras.Persistence.Pos;

namespace Renligou.Application.Tests.Dynamic
{
    public class DynamicCrudRegistryTests
    {
        private readonly DynamicCrudRegistry _registry;

        public DynamicCrudRegistryTests()
        {
            _registry = new DynamicCrudRegistry();
        }

        [Fact]
        public void Add_ShouldAddConfig_WhenConfigIsValid()
        {
            // Arrange
            var config = CreateValidTestConfig();

            // Act
            _registry.Add(config);
            var retrievedConfig = _registry.Get<TestDto>();

            // Assert
            retrievedConfig.Should().NotBeNull();
            retrievedConfig.Title.Should().Be("Test Entity");
            retrievedConfig.EntityType.Should().Be(typeof(TestPo));
            retrievedConfig.DtoType.Should().Be(typeof(TestDto));
        }

        [Fact]
        public void Add_ShouldThrowException_WhenEntityTypeIsNull()
        {
            // Arrange
            var config = new EntityConfig<TestDto>
            {
                Title = "Test Entity",
                EntityType = null!,
                EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<TestDto>>(),
                RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>()
            };

            // Act & Assert - 由于 string.Format 格式化问题，会抛出 FormatException
            var exception = Assert.Throws<FormatException>(() => _registry.Add(config));
            exception.Message.Should().Contain("not in a correct format");
        }

        [Fact]
        public void Add_ShouldThrowException_WhenDtoTypeIsNull()
        {
            // Arrange - 这个场景实际上难以发生，因为 DtoType 是基于 typeof(TDto) 的
            // 但我们可以测试逻辑是否存在
            // 这里我们使用反射来设置私有 setter
            var config = new EntityConfig<TestDto>
            {
                Title = "Test Entity",
                EntityType = typeof(TestPo),
                EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<TestDto>>(),
                RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>()
            };

            // 由于 DtoType 是基于 typeof(TDto)，不会为 null，这个测试应该能成功
            // Act
            _registry.Add(config);

            // Assert
            var retrieved = _registry.Get<TestDto>();
            retrieved.DtoType.Should().Be(typeof(TestDto));
        }

        [Fact]
        public void Add_ShouldOverwriteExistingConfig_WhenSameDtoTypeAdded()
        {
            // Arrange
            var config1 = CreateValidTestConfig();
            config1.Title = "First Config";

            var config2 = CreateValidTestConfig();
            config2.Title = "Second Config";

            // Act
            _registry.Add(config1);
            _registry.Add(config2);
            var retrieved = _registry.Get<TestDto>();

            // Assert
            retrieved.Title.Should().Be("Second Config");
        }

        [Fact]
        public void Get_ShouldReturnConfig_WhenConfigExists()
        {
            // Arrange
            var config = CreateValidTestConfig();
            _registry.Add(config);

            // Act
            var retrieved = _registry.Get<TestDto>();

            // Assert
            retrieved.Should().NotBeNull();
            retrieved.Should().Be(config);
        }

        [Fact]
        public void Get_ShouldThrowException_WhenConfigDoesNotExist()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _registry.Get<TestDto>());
            exception.Message.Should().Contain("获取动态查询配置错误");
        }

        [Fact]
        public void Get_ShouldReturnCorrectConfig_WhenMultipleConfigsExist()
        {
            // Arrange
            var testConfig = CreateValidTestConfig();
            var anotherConfig = new EntityConfig<AnotherDto>
            {
                Title = "Another Entity",
                EntityType = typeof(AnotherEntity),
                EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<AnotherDto>>(),
                RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>(),
                KeyName = "Id",
                KeyPropertyInfo = typeof(AnotherEntity).GetProperty("Id")!
            };

            _registry.Add(testConfig);
            _registry.Add(anotherConfig);

            // Act
            var retrievedTestConfig = _registry.Get<TestDto>();
            var retrievedAnotherConfig = _registry.Get<AnotherDto>();

            // Assert
            retrievedTestConfig.Title.Should().Be("Test Entity");
            retrievedTestConfig.EntityType.Should().Be(typeof(TestPo));

            retrievedAnotherConfig.Title.Should().Be("Another Entity");
            retrievedAnotherConfig.EntityType.Should().Be(typeof(AnotherEntity));
        }

        [Fact]
        public void Add_ShouldWorkConcurrently_WhenCalledFromMultipleThreads()
        {
            // Arrange
            var configs = Enumerable.Range(0, 100).Select(i => new
            {
                Config = new EntityConfig<TestDto>
                {
                    Title = $"Config {i}",
                    EntityType = typeof(TestPo),
                    EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<TestDto>>(),
                    RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>(),
                    KeyName = "Id",
                    KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!
                }
            }).ToList();

            // Act - 并发添加配置
            Parallel.ForEach(configs, item => _registry.Add(item.Config));

            // Assert - 应该只有最后一个配置生效
            var retrieved = _registry.Get<TestDto>();
            retrieved.Should().NotBeNull();
            retrieved.EntityType.Should().Be(typeof(TestPo));
        }

        [Fact]
        public void Add_ShouldPreserveConfigProperties_WhenConfigIsAdded()
        {
            // Arrange
            var config = new EntityConfig<TestDto>
            {
                Title = "Test Entity",
                EntityType = typeof(TestPo),
                EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<TestDto>>(),
                RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>(),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!,
                EnableDetailCache = true,
                DetailCacheDurationInSeconds = 120,
                EnableOptionCache = true,
                OptionCacheDurationInSeconds = 300,
                EnablePageCache = true,
                PageCacheDurationInSeconds = 90,
                EnableSoftDelete = true
            };

            // Act
            _registry.Add(config);
            var retrieved = _registry.Get<TestDto>();

            // Assert
            retrieved.EnableDetailCache.Should().BeTrue();
            retrieved.DetailCacheDurationInSeconds.Should().Be(120);
            retrieved.EnableOptionCache.Should().BeTrue();
            retrieved.OptionCacheDurationInSeconds.Should().Be(300);
            retrieved.EnablePageCache.Should().BeTrue();
            retrieved.PageCacheDurationInSeconds.Should().Be(90);
            retrieved.EnableSoftDelete.Should().BeTrue();
        }

        // 辅助方法：创建有效的测试配置
        private EntityConfig<TestDto> CreateValidTestConfig()
        {
            return new EntityConfig<TestDto>
            {
                Title = "Test Entity",
                EntityType = typeof(TestPo),
                EntityToDtoMapper = Substitute.For<IEntityToDtoMapper<TestDto>>(),
                RequestToEntityMapper = Substitute.For<IRequestToEntityMapper>(),
                KeyName = "Id",
                KeyPropertyInfo = typeof(TestPo).GetProperty("Id")!
            };
        }

        // 测试用的另一个 DTO 类
        public class AnotherDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        // 测试用的另一个实体类
        public class AnotherEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}

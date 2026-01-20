using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Criterias;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Renligou.Core.Application.Tests.IdentityAccess
{
    [TestFixture]
    public class GetPermissionGroupTreeHandlerTests
    {
        private Mock<IPermissionGroupQueryRepository> _queryRepositoryMock;
        private GetPermissionGroupTreeHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _queryRepositoryMock = new Mock<IPermissionGroupQueryRepository>();
            _handler = new GetPermissionGroupTreeHandler(_queryRepositoryMock.Object);
        }

        [Test]
        public async Task HandleAsync_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var query = new GetPermissionGroupTreeQuery
            {
                ParentId = 10,
                Name = "Test"
            };
            var cancellationToken = new CancellationToken();
            var expectedNodes = new List<PermissionGroupTreeDto>();

            _queryRepositoryMock
                .Setup(x => x.GetPermissionGroupTreeAsync(It.IsAny<PermissionGroupTreeCriteria>(), cancellationToken))
                .ReturnsAsync(expectedNodes);

            // Act
            var result = await _handler.HandleAsync(query, cancellationToken);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Value, Is.SameAs(expectedNodes));
            _queryRepositoryMock.Verify(x => x.GetPermissionGroupTreeAsync(
                It.Is<PermissionGroupTreeCriteria>(c => c.ParentId == 10 && c.Name == "Test"),
                cancellationToken), Times.Once);
        }

        [Test]
        public async Task HandleAsync_ShouldPropagateCancellationToken()
        {
            // Arrange
            var query = new GetPermissionGroupTreeQuery();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            _queryRepositoryMock
                .Setup(x => x.GetPermissionGroupTreeAsync(It.IsAny<PermissionGroupTreeCriteria>(), cts.Token))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () => 
                await _handler.HandleAsync(query, cts.Token));
        }

        [Test]
        public async Task HandleAsync_ShouldHandleConcurrentDispatches()
        {
            // Arrange
            var query1 = new GetPermissionGroupTreeQuery { ParentId = 1 };
            var query2 = new GetPermissionGroupTreeQuery { ParentId = 2 };

            _queryRepositoryMock
                .Setup(x => x.GetPermissionGroupTreeAsync(It.Is<PermissionGroupTreeCriteria>(c => c.ParentId == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PermissionGroupTreeDto> { new PermissionGroupTreeDto { Id = 1 } });

            _queryRepositoryMock
                .Setup(x => x.GetPermissionGroupTreeAsync(It.Is<PermissionGroupTreeCriteria>(c => c.ParentId == 2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PermissionGroupTreeDto> { new PermissionGroupTreeDto { Id = 2 } });

            // Act
            var task1 = _handler.HandleAsync(query1, CancellationToken.None);
            var task2 = _handler.HandleAsync(query2, CancellationToken.None);

            var results = await Task.WhenAll(task1, task2);

            // Assert
            Assert.That(results[0].Value[0].Id, Is.EqualTo(1));
            Assert.That(results[1].Value[0].Id, Is.EqualTo(2));
        }
    }
}

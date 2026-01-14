using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Application.Enterprise.Handlers;
using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Model;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.Enterprise;

[TestFixture]
public class CreateCompanyHandlerTests
{
    private MockCompanyRepository _companyRepository;
    private MockRegionQueryRepository _regionQueryRepository;
    private MockOutboxRepository _outboxRepository;
    private MockIdGenerator _idGenerator;
    private CreateCompanyHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _companyRepository = new MockCompanyRepository();
        _regionQueryRepository = new MockRegionQueryRepository();
        _outboxRepository = new MockOutboxRepository();
        _idGenerator = new MockIdGenerator();
        _handler = new CreateCompanyHandler(
            _companyRepository,
            _regionQueryRepository,
            _outboxRepository,
            _idGenerator
        );
    }

    [Test]
    public async Task HandleAsync_WhenValidCommand_ShouldSaveCompanyAndOutbox()
    {
        // Arrange
        var command = new CreateCompanyCommand
        {
            CompanyType = "HEADQUARTERS",
            CompanyCode = "C001",
            CompanyName = "Test Company",
            ProvinceId = 1,
            CityId = 2,
            DistrictId = 3,
            CompletedAddress = "Street 1",
            Enabled = true
        };
        _idGenerator.IdToReturn = 123;
        _regionQueryRepository.NamesToReturn[1] = "P1";
        _regionQueryRepository.NamesToReturn[2] = "C2";
        _regionQueryRepository.NamesToReturn[3] = "D3";

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(_companyRepository.SavedAggregate, Is.Not.Null);
        Assert.That(_companyRepository.SavedAggregate!.Id.id, Is.EqualTo(123));
        Assert.That(_companyRepository.SavedAggregate.CompanyName, Is.EqualTo("Test Company"));
        
        // Final verification for the issue: Check if events were passed to Outbox
        Assert.That(_outboxRepository.AddedEvents, Has.Count.GreaterThan(0));
        Assert.That(_outboxRepository.AddedEvents[0].AggregateId, Is.EqualTo("123"));
    }

    [Test]
    public async Task HandleAsync_WhenNameExists_ShouldReturnFail()
    {
        // Arrange
        var command = new CreateCompanyCommand { CompanyName = "Existing" };
        _companyRepository.NameExistsResponse = true;

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Code, Is.EqualTo("Company.Create.Error"));
        Assert.That(_companyRepository.SavedAggregate, Is.Null);
    }

    #region Mocks

    private class MockCompanyRepository : ICompanyRepository
    {
        public Company? SavedAggregate { get; private set; }
        public bool NameExistsResponse { get; set; }

        public Task<Company?> LoadAsync(long id) => Task.FromResult<Company?>(null);
        public Task SaveAsync(Company aggregate) { SavedAggregate = aggregate; return Task.CompletedTask; }
        public Task<bool> CompanyNameExistsAsync(long companyId, string companyName) => Task.FromResult(NameExistsResponse);
    }

    private class MockRegionQueryRepository : IRegionQueryRepository
    {
        public Dictionary<long, string> NamesToReturn { get; } = new();

        public Task<List<RegionDto>> QeuryResionListAsync(long parentId, string? regionName)
        {
            return Task.FromResult(new List<RegionDto>());
        }

        public Task<Dictionary<long, string>> QueryRegionNamesByIdsAsync(IEnumerable<long> regionIds) => Task.FromResult(NamesToReturn);
    }

    private class MockOutboxRepository : IOutboxRepository
    {
        public List<(IEnumerable<IIntegrationEvent> Events, string Category, string AggregateType, string AggregateId)> AddedEvents { get; } = new();

        public Task AddAsync(IIntegrationEvent @event, string category, string aggregateType, string aggregateId)
        {
            AddedEvents.Add((new[] { @event }, category, aggregateType, aggregateId));
            return Task.CompletedTask;
        }

        public Task AddAsync(IEnumerable<IIntegrationEvent> events, string category, string aggregateType, string aggregateId)
        {
            AddedEvents.Add((events, category, aggregateType, aggregateId));
            return Task.CompletedTask;
        }
    }

    private class MockIdGenerator : IIdGenerator
    {
        public long IdToReturn { get; set; }
        public long NextId() => IdToReturn;
    }

    #endregion
}

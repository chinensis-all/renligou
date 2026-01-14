namespace Renligou.Core.Domain.CommonContext.Repo
{
    public interface DomainRepository<IAggregate> where IAggregate : class
    {
        Task<IAggregate?> LoadAsync(long id);

        Task SaveAsync(IAggregate aggregate);
    }
}

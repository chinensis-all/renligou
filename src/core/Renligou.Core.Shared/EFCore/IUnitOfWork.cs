namespace Renligou.Core.Shared.EFCore
{
    public interface IUnitOfWork
    {
        Task BeginAsync();

        Task CommitAsync();

        Task RollbackAsync();

        Task SaveChangesAsync();

        Task ExecuteAsync(Func<Task> action, bool transactional = true);

        Task<T> ExecuteAsync<T>(Func<Task<T>> action, bool transactional = true);
    }
}

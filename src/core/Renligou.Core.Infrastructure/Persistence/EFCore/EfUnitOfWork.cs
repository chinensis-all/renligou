using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Core.Infrastructure.Persistence.EFCore
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _db;

        private IDbContextTransaction? _tx;

        public EfUnitOfWork(DbContext db)
        {
            _db = db;
        }

        public async Task BeginAsync()
        {
            _tx = await _db.Database.BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            if (_tx != null)
                await _tx.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_tx != null)
                await _tx.RollbackAsync();
        }

        public async Task ExecuteAsync(Func<Task> action, bool transactional = true)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                if (!transactional)
                {
                    await action();
                    await _db.SaveChangesAsync();
                    return;
                }

                await using var tx = await _db.Database.BeginTransactionAsync();
                try
                {
                    await action();
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, bool transactional = true)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                if (!transactional)
                {
                    var r = await action();
                    await _db.SaveChangesAsync();
                    return r;
                }

                await using var tx = await _db.Database.BeginTransactionAsync();
                try
                {
                    var r = await action();
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                    return r;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });
        }
    }
}

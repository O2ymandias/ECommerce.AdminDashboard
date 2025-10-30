using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Models;

namespace ECommerce.Core.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ModelBase;
    IIdentityRepository IdentityRepository { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Models;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Infrastructure.Database;
using ECommerce.Infrastructure.Repositories.GenericRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure;

public class UnitOfWork(AppDbContext dbContext, UserManager<AppUser> userManager) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repos = [];

    public IIdentityRepository IdentityRepository
    {
        get
        {
            var key = typeof(IIdentityRepository);
            if (_repos.TryGetValue(key, out var retrieved))
                return (IIdentityRepository)retrieved;
            var repo = new IdentityRepository(dbContext, userManager);
            _repos.Add(key, repo);
            return repo;
        }
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ModelBase
    {
        var key = typeof(TEntity);

        if (_repos.TryGetValue(key, out var retrieved))
            return (IGenericRepository<TEntity>)retrieved;

        var repo = new GenericRepository<TEntity>(dbContext);
        _repos.Add(key, repo);
        return repo;
    }


    public Task<int> SaveChangesAsync() =>
        dbContext.SaveChangesAsync();

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await dbContext.DisposeAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null) return;

        _transaction = await dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction is not null) await _transaction.CommitAsync();
        }
        catch (Exception)
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
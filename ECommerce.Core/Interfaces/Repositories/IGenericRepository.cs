using ECommerce.Core.Models;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : ModelBase
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity>? specs);
    IQueryable<TEntity> GetAllAsQueryable(ISpecification<TEntity>? specs);
    Task<TEntity?> GetAsync(ISpecification<TEntity> specs, bool checkLocalCache = true);
    Task<int> CountAsync(ISpecification<TEntity>? specs);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
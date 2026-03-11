using BuildingBlocks.Domain.Repositories;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Persistence.EfCore.DbContexts;

namespace BuildingBlocks.Persistence.EfCore.Repositories;

public class EfCoreWriteRepository<TEntity, TId, TDbContext>(TDbContext dbContext) : EfCoreRepositoryBase<TEntity, TDbContext>(dbContext), IWriteRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : struct
    where TDbContext : EfCoreDbContext
{
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await DbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity)
        => DbSet.Update(entity);

    public void Remove(TEntity entity)
        => DbSet.Remove(entity);
}
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Application.Repositories;

public interface IWriteRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : struct
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

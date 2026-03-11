using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Domain.Checks;

[Obsolete("Use IPolicy<TContext> from BuildingBlocks.Application.Policies instead.")]
public interface IConstraintCheck<in TEntity, in TId>
    where TEntity : IEntity<TId>
    where TId : struct
{
    Task<Result> EnsureExists(TId id, CancellationToken ct = default);
    Task<Result> EnsureCanCreate(TEntity entity, CancellationToken ct = default);
    Task<Result> EnsureCanUpdate(TEntity entity, CancellationToken ct = default);
    Task<Result> EnsureCanDelete(TId id, CancellationToken ct = default);
}

using BuildingBlocks.Domain.Checks;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Application.Checks;

[Obsolete("Use PolicyBase<TContext> from BuildingBlocks.Application.Policies instead.")]
public abstract class ConstraintCheckBase<TEntity, TId> : IConstraintCheck<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : struct
{
    private static readonly Task<Result> SuccessResult = Task.FromResult(ResultCreator.Success());

    public abstract Task<Result> EnsureExists(TId id, CancellationToken ct = default);

    public virtual Task<Result> EnsureCanCreate(TEntity entity, CancellationToken ct = default)
        => SuccessResult;

    public virtual Task<Result> EnsureCanUpdate(TEntity entity, CancellationToken ct = default)
        => SuccessResult;

    public virtual Task<Result> EnsureCanDelete(TId id, CancellationToken ct = default)
        => SuccessResult;
}
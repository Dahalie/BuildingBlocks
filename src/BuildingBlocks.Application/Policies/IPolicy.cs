using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Application.Policies;

/// <summary>
/// Marker interface for DI assembly scanning.
/// </summary>
public interface IPolicy;

/// <summary>
/// Precondition policy that evaluates business rules for a given context.
/// Returns <see cref="Result"/> indicating whether the operation can proceed.
/// </summary>
public interface IPolicy<in TContext> : IPolicy
{
    Task<Result> ApplyAsync(TContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Decision/calculation policy that evaluates business rules and produces a result value.
/// Returns <see cref="Result{TResult}"/> containing the computed outcome.
/// </summary>
public interface IPolicy<in TContext, TResult> : IPolicy
{
    Task<Result<TResult>> ApplyAsync(TContext context, CancellationToken cancellationToken = default);
}

using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Application.Policies;

public abstract class PolicyBase<TContext> : IPolicy<TContext>
{
    public abstract Task<Result> ApplyAsync(TContext context, CancellationToken cancellationToken = default);
}

public abstract class PolicyBase<TContext, TResult> : IPolicy<TContext, TResult>
{
    public abstract Task<Result<TResult>> ApplyAsync(TContext context, CancellationToken cancellationToken = default);
}

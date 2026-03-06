namespace BuildingBlocks.Application.Mediator.Requests;

public abstract record QueryBase<TResponse> : IQuery<TResponse>;
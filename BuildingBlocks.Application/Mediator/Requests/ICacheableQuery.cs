namespace BuildingBlocks.Application.Mediator.Requests;

public interface ICacheableQuery
{
    string    CacheKey   { get; }
    TimeSpan? Expiration { get; }
}
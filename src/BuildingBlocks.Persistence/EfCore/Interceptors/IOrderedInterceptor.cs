namespace BuildingBlocks.Persistence.EfCore.Interceptors;

public interface IOrderedInterceptor
{
    int Order { get; }
}

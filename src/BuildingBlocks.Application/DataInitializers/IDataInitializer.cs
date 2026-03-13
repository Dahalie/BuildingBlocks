namespace BuildingBlocks.Application.DataInitializers;

public interface IDataInitializer
{
    int Order { get; }

    Task InitializeAsync(CancellationToken ct = default);
}

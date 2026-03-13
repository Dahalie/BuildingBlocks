using BuildingBlocks.Application.DataInitializers;

namespace BuildingBlocks.Application.Tests.DataInitializers;

public class FakeDataInitializer : IDataInitializer
{
    public int Order => 1;
    public bool Initialized { get; private set; }

    public Task InitializeAsync(CancellationToken ct = default)
    {
        Initialized = true;
        return Task.CompletedTask;
    }
}

public class SecondFakeDataInitializer : IDataInitializer
{
    public int Order => 2;
    public bool Initialized { get; private set; }

    public Task InitializeAsync(CancellationToken ct = default)
    {
        Initialized = true;
        return Task.CompletedTask;
    }
}

public abstract class AbstractFakeDataInitializer : IDataInitializer
{
    public int Order => 0;
    public abstract Task InitializeAsync(CancellationToken ct = default);
}

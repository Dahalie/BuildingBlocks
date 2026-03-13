using BuildingBlocks.Application.DataInitializers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Tests.DataInitializers;

public class DataInitializerMicrosoftExtensionsTests
{
    [Fact]
    public void AddDataInitializersFromAssemblies_RegistersInitializer()
    {
        var services = new ServiceCollection();
        services.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var provider = services.BuildServiceProvider();

        var initializers = provider.GetServices<IDataInitializer>();

        initializers.Should().ContainSingle(x => x.GetType() == typeof(FakeDataInitializer));
    }

    [Fact]
    public void AddDataInitializersFromAssemblies_RegistersMultipleInitializers()
    {
        var services = new ServiceCollection();
        services.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var provider = services.BuildServiceProvider();

        var initializers = provider.GetServices<IDataInitializer>().ToList();

        initializers.Should().HaveCount(2);
    }

    [Fact]
    public void AddDataInitializersFromAssemblies_DoesNotRegisterDuplicates_WhenCalledTwice()
    {
        var services = new ServiceCollection();
        services.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        services.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var provider = services.BuildServiceProvider();

        var initializers = provider.GetServices<IDataInitializer>().ToList();

        initializers.Should().HaveCount(2);
    }
}

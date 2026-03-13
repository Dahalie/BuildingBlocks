using Autofac;
using BuildingBlocks.Application.DataInitializers;
using FluentAssertions;

namespace BuildingBlocks.Application.Tests.DataInitializers;

public class DataInitializerAutofacExtensionsTests
{
    [Fact]
    public void AddDataInitializersFromAssemblies_RegistersInitializer_AsInterface()
    {
        var builder = new ContainerBuilder();
        builder.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var container = builder.Build();

        var initializers = container.Resolve<IEnumerable<IDataInitializer>>();

        initializers.Should().ContainSingle(x => x.GetType() == typeof(FakeDataInitializer));
    }

    [Fact]
    public void AddDataInitializersFromAssemblies_DoesNotRegisterAbstractInitializers()
    {
        var builder = new ContainerBuilder();
        builder.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var container = builder.Build();

        var resolved = container.ResolveOptional<AbstractFakeDataInitializer>();

        resolved.Should().BeNull();
    }

    [Fact]
    public void AddDataInitializersFromAssemblies_RegistersMultipleInitializers()
    {
        var builder = new ContainerBuilder();
        builder.AddDataInitializersFromAssemblies(typeof(FakeDataInitializer).Assembly);
        using var container = builder.Build();

        var initializers = container.Resolve<IEnumerable<IDataInitializer>>().ToList();

        initializers.Should().HaveCount(2);
    }
}

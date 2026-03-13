using BuildingBlocks.Api.DataInitializers;
using BuildingBlocks.Application.DataInitializers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace BuildingBlocks.Api.Tests.DataInitializers;

public class DataInitializerHostExtensionsTests
{
    [Fact]
    public async Task RunDataInitializersAsync_ExecutesAllInitializers()
    {
        var first = new TrackingInitializer(1);
        var second = new TrackingInitializer(2);
        var host = CreateHost(first, second);

        await host.RunDataInitializersAsync();

        first.Initialized.Should().BeTrue();
        second.Initialized.Should().BeTrue();
    }

    [Fact]
    public async Task RunDataInitializersAsync_ExecutesInOrder()
    {
        var executionOrder = new List<int>();
        var second = new TrackingInitializer(2, executionOrder);
        var first = new TrackingInitializer(1, executionOrder);
        var host = CreateHost(second, first);

        await host.RunDataInitializersAsync();

        executionOrder.Should().ContainInOrder(1, 2);
    }

    [Fact]
    public async Task RunDataInitializersAsync_DoesNothing_WhenNoInitializersRegistered()
    {
        var host = CreateHost();

        var act = () => host.RunDataInitializersAsync();

        await act.Should().NotThrowAsync();
    }

    private static IHost CreateHost(params IDataInitializer[] initializers)
    {
        var services = new ServiceCollection();

        foreach (var initializer in initializers)
        {
            services.AddSingleton(typeof(IDataInitializer), initializer);
        }

        var serviceProvider = services.BuildServiceProvider();
        var host = Substitute.For<IHost>();
        host.Services.Returns(serviceProvider);

        return host;
    }

    private class TrackingInitializer : IDataInitializer
    {
        private readonly List<int>? _executionOrder;

        public TrackingInitializer(int order, List<int>? executionOrder = null)
        {
            Order = order;
            _executionOrder = executionOrder;
        }

        public int Order { get; }
        public bool Initialized { get; private set; }

        public Task InitializeAsync(CancellationToken ct = default)
        {
            Initialized = true;
            _executionOrder?.Add(Order);
            return Task.CompletedTask;
        }
    }
}

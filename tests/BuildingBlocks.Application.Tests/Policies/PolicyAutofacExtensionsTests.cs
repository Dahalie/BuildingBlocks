using System.Reflection;
using Autofac;
using BuildingBlocks.Application.Policies;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;

namespace BuildingBlocks.Application.Tests.Policies;

public class PolicyAutofacExtensionsTests
{
    [Fact]
    public void AddPoliciesFromAssemblies_RegistersPreconditionPolicy_AsInterface()
    {
        var builder = new ContainerBuilder();
        builder.AddPoliciesFromAssemblies(typeof(FakePreconditionPolicy).Assembly);
        using var container = builder.Build();

        var policy = container.Resolve<IPolicy<FakeCommand>>();

        policy.Should().BeOfType<FakePreconditionPolicy>();
    }

    [Fact]
    public void AddPoliciesFromAssemblies_RegistersPreconditionPolicy_AsSelf()
    {
        var builder = new ContainerBuilder();
        builder.AddPoliciesFromAssemblies(typeof(FakePreconditionPolicy).Assembly);
        using var container = builder.Build();

        var policy = container.Resolve<FakePreconditionPolicy>();

        policy.Should().NotBeNull();
    }

    [Fact]
    public void AddPoliciesFromAssemblies_RegistersDecisionPolicy_AsInterface()
    {
        var builder = new ContainerBuilder();
        builder.AddPoliciesFromAssemblies(typeof(FakeDecisionPolicy).Assembly);
        using var container = builder.Build();

        var policy = container.Resolve<IPolicy<FakeCommand, decimal>>();

        policy.Should().BeOfType<FakeDecisionPolicy>();
    }

    [Fact]
    public void AddPoliciesFromAssemblies_RegistersDecisionPolicy_AsSelf()
    {
        var builder = new ContainerBuilder();
        builder.AddPoliciesFromAssemblies(typeof(FakeDecisionPolicy).Assembly);
        using var container = builder.Build();

        var policy = container.Resolve<FakeDecisionPolicy>();

        policy.Should().NotBeNull();
    }

    [Fact]
    public void AddPoliciesFromAssemblies_DoesNotRegisterAbstractPolicies()
    {
        var builder = new ContainerBuilder();
        builder.AddPoliciesFromAssemblies(typeof(FakePreconditionPolicy).Assembly);
        using var container = builder.Build();

        var resolved = container.ResolveOptional<AbstractFakePolicy>();

        resolved.Should().BeNull();
    }

    [Fact]
    public async Task PreconditionPolicy_ApplyAsync_ReturnsExpectedResult()
    {
        var policy = new FakePreconditionPolicy();

        var result = await policy.ApplyAsync(new FakeCommand(true));

        result.IsSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task PreconditionPolicy_ApplyAsync_ReturnsFailure_WhenConditionNotMet()
    {
        var policy = new FakePreconditionPolicy();

        var result = await policy.ApplyAsync(new FakeCommand(false));

        result.IsFailed.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.Business);
    }

    [Fact]
    public async Task DecisionPolicy_ApplyAsync_ReturnsComputedValue()
    {
        var policy = new FakeDecisionPolicy();

        var result = await policy.ApplyAsync(new FakeCommand(true));

        result.IsSucceeded.Should().BeTrue();
        result.DataOrDefault.Should().Be(0.15m);
    }
}

// ── Test fakes ──

public record FakeCommand(bool IsValid);

public class FakePreconditionPolicy : PolicyBase<FakeCommand>
{
    public override Task<Result> ApplyAsync(FakeCommand context, CancellationToken cancellationToken = default)
    {
        return context.IsValid
            ? Task.FromResult(ResultCreator.Success())
            : Task.FromResult(ResultCreator.Fail(ErrorCreator.Business("Fake.Invalid", "Invalid.")));
    }
}

public class FakeDecisionPolicy : PolicyBase<FakeCommand, decimal>
{
    public override Task<Result<decimal>> ApplyAsync(FakeCommand context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ResultCreator.Success(0.15m));
    }
}

public abstract class AbstractFakePolicy : PolicyBase<FakeCommand>
{
}

using BuildingBlocks.Application.Mediator.Requests;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace BuildingBlocks.Application.Tests.Mediator.Requests;

public record TestQuery : QueryBase<string>;

public class RequestDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_SendsViaMediatR()
    {
        var sender = Substitute.For<ISender>();
        IRequestDispatcher dispatcher = new RequestDispatcher(sender);
        var query = new TestQuery();
        var expected = ResultCreator.Success<string>("result");

        sender.Send(query, Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));

        var result = await dispatcher.DispatchAsync(query);

        result.Should().Be(expected);
        await sender.Received(1).Send(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void QueryBase_ImplementsIQuery()
    {
        var query = new TestQuery();

        query.Should().BeAssignableTo<IQuery<string>>();
    }
}

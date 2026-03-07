using BuildingBlocks.Application.Mediator.Behaviors;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BuildingBlocks.Application.Tests.Mediator.Behaviors;

public record TestLoggingCommand : IRequest<Result>;

public record TestLoggingQuery : IRequest<Result<string>>;

public class RequestLoggingBehaviorTests
{
    private readonly ILogger<RequestLoggingBehavior<TestLoggingCommand, Result>> _commandLogger = Substitute.For<ILogger<RequestLoggingBehavior<TestLoggingCommand, Result>>>();
    private readonly ILogger<RequestLoggingBehavior<TestLoggingQuery, Result<string>>> _queryLogger = Substitute.For<ILogger<RequestLoggingBehavior<TestLoggingQuery, Result<string>>>>();

    [Fact]
    public async Task Handle_SuccessfulRequest_LogsInformation()
    {
        var behavior = new RequestLoggingBehavior<TestLoggingCommand, Result>(_commandLogger);
        var result   = ResultCreator.Success();

        var response = await behavior.Handle(new TestLoggingCommand(), _ => Task.FromResult(result), CancellationToken.None);

        response.IsSucceeded.Should().BeTrue();

        _commandLogger.Received(2).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_FailedWithExceptionErrorType_LogsWarning()
    {
        var behavior = new RequestLoggingBehavior<TestLoggingCommand, Result>(_commandLogger);
        var error    = ErrorCreator.Create(ErrorType.Exception, "Test.Exception", "Something went wrong");
        var result   = ResultCreator.Fail(error);

        var response = await behavior.Handle(new TestLoggingCommand(), _ => Task.FromResult(result), CancellationToken.None);

        response.IsFailed.Should().BeTrue();

        _commandLogger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_FailedWithNonExceptionErrorType_LogsInformation()
    {
        var behavior = new RequestLoggingBehavior<TestLoggingCommand, Result>(_commandLogger);
        var error    = ErrorCreator.Create(ErrorType.NotFound, "Test.NotFound", "Not found");
        var result   = ResultCreator.Fail(error);

        var response = await behavior.Handle(new TestLoggingCommand(), _ => Task.FromResult(result), CancellationToken.None);

        response.IsFailed.Should().BeTrue();

        _commandLogger.Received(2).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());

        _commandLogger.DidNotReceive().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_GenericResult_WorksCorrectly()
    {
        var behavior = new RequestLoggingBehavior<TestLoggingQuery, Result<string>>(_queryLogger);
        var result = ResultCreator.Success<string>("data");

        var response = await behavior.Handle(new TestLoggingQuery(), _ => Task.FromResult<Result<string>>(result), CancellationToken.None);

        response.IsSucceeded.Should().BeTrue();
        response.DataOrThrow.Should().Be("data");
    }
}

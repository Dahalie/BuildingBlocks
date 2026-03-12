using BuildingBlocks.Primitives.Results;

namespace BuildingBlocks.Infrastructure.Messaging;

public sealed class IntegrationEventProcessingException(Error error)
    : Exception($"Integration event processing failed: [{error.ErrorCode}] {error.ErrorMessage}")
{
    public Error Error { get; } = error;
}

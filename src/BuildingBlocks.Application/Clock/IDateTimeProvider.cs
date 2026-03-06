namespace BuildingBlocks.Application.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
using BuildingBlocks.Application.Clock;

namespace BuildingBlocks.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
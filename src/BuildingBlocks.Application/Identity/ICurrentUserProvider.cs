namespace BuildingBlocks.Application.Identity;

public interface ICurrentUserProvider
{
    Guid UserId { get; }
}
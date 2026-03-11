namespace BuildingBlocks.Application.Identity;

public interface ICurrentUserProvider<TUserId> where TUserId : struct
{
    TUserId UserId { get; }
}
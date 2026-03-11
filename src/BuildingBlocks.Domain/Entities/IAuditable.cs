namespace BuildingBlocks.Domain.Entities;

public interface IAuditable<TUserId> where TUserId : struct
{
    TUserId  CreatedBy { get; set; }
    TUserId? UpdatedBy { get; set; }
}
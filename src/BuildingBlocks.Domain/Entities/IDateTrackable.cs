namespace BuildingBlocks.Domain.Entities;

public interface IDateTrackable
{
    DateTimeOffset  CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
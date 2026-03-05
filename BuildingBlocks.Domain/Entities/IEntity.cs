namespace BuildingBlocks.Domain.Entities;

public interface IEntity
{
}

public interface IEntity<TId> : IEntity
    where TId : struct
{
    TId Id { get; set; }
}
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace BuildingBlocks.Persistence.EfCore.ValueGenerators;

public class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry) => Guid.CreateVersion7();

    public override bool GeneratesTemporaryValues => false;
}

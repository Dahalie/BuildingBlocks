using BuildingBlocks.Persistence.EfCore.ValueGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BuildingBlocks.Persistence.EfCore.Conventions;

public class GuidV7Convention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        var generator = new GuidV7ValueGenerator();

        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var primaryKey = entityType.FindPrimaryKey();

            if (primaryKey is { Properties.Count: 1 } && primaryKey.Properties[0].ClrType == typeof(Guid))
            {
                primaryKey.Properties[0].SetValueGeneratorFactory((_, _) => generator);
            }
        }
    }
}

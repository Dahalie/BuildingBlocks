using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Conventions;

public static class GuidV7ModelConfigurationExtensions
{
    public static ModelConfigurationBuilder AddGuidV7Convention(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new GuidV7Convention());
        return configurationBuilder;
    }
}

using BuildingBlocks.Primitives.Types;

namespace BuildingBlocks.Persistence.EfCore.Resources;

public static class DbContextMessages
{
    public static string CouldNotCreateInstance(Type type)
        => $"Could not create an instance of {type.MapToReadableString()}.";
}
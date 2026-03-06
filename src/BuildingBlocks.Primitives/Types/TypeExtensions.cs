namespace BuildingBlocks.Primitives.Types;

public static class TypeExtensions
{
    extension(Type type)
    {
        public string MapToReadableString()
        {
            var name          = type.Name;
            var backtickIndex = name.IndexOf('`');
            var baseName      = backtickIndex > 0 ? name[..backtickIndex] : name;

            if (!type.IsGenericType) return baseName;

            var genericArgs = type.GetGenericArguments().Select(t => t.MapToReadableString());

            return $"{baseName}<{string.Join(", ", genericArgs)}>";
        }
    }
}
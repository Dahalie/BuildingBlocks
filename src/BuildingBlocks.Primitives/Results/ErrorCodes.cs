namespace BuildingBlocks.Primitives.Results;

public static class ErrorCodes
{
    public const string Multiple = "Multiple";

    public static string Validation(string instanceName)
        => $"{instanceName}.Validation";

    public static string NotFound(string instanceName)
        => $"{instanceName}.NotFound";
}
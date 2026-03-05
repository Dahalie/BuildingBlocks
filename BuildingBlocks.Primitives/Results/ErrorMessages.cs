namespace BuildingBlocks.Primitives.Results;

public static class ErrorMessages
{
    public const string MultipleError = "Multiple errors occurred. ";

    public const string Validation = "One or more validation error occurred. ";

    public const string None = "None";

    public const string NoError = "No error.";

    public static string NotFound(string instanceName)
        => $"No {instanceName} was found.";
}
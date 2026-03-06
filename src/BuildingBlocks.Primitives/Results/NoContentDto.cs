using System.Text.Json.Serialization;

namespace BuildingBlocks.Primitives.Results;

public sealed class NoContentDto
{
    private static NoContentDto? _instance;

    [JsonConstructor]
    private NoContentDto()
    {
    }

    public static NoContentDto Instance => _instance ??= new();
}
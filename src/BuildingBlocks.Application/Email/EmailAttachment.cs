namespace BuildingBlocks.Application.Email;

public sealed class EmailAttachment
{
    public required string FileName { get; init; }
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
}

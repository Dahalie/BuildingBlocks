namespace BuildingBlocks.Application.Email;

public sealed class EmailMessage
{
    public EmailAddress? From { get; init; }
    public required List<EmailAddress> To { get; init; }
    public List<EmailAddress>? Cc { get; init; }
    public List<EmailAddress>? Bcc { get; init; }
    public EmailAddress? ReplyTo { get; init; }
    public required string Subject { get; init; }
    public string? HtmlBody { get; init; }
    public string? TextBody { get; init; }
    public List<EmailAttachment>? Attachments { get; init; }
}

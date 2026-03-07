using BuildingBlocks.Application.Email;
using FluentAssertions;

namespace BuildingBlocks.Application.Tests.Email;

public class EmailMessageTests
{
    [Fact]
    public void Constructor_WithRequiredProperties_SetsValues()
    {
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test Subject",
            TextBody = "Hello"
        };

        message.To.Should().HaveCount(1);
        message.Subject.Should().Be("Test Subject");
        message.TextBody.Should().Be("Hello");
    }

    [Fact]
    public void OptionalProperties_DefaultToNull()
    {
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test"
        };

        message.From.Should().BeNull();
        message.Cc.Should().BeNull();
        message.Bcc.Should().BeNull();
        message.ReplyTo.Should().BeNull();
        message.HtmlBody.Should().BeNull();
        message.TextBody.Should().BeNull();
        message.Attachments.Should().BeNull();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        using var stream = new MemoryStream([1, 2, 3]);

        var message = new EmailMessage
        {
            From = new EmailAddress("from@example.com", "Sender"),
            To = [new EmailAddress("to@example.com")],
            Cc = [new EmailAddress("cc@example.com")],
            Bcc = [new EmailAddress("bcc@example.com")],
            ReplyTo = new EmailAddress("reply@example.com"),
            Subject = "Full Message",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello",
            Attachments =
            [
                new EmailAttachment
                {
                    FileName = "file.txt",
                    Content = stream,
                    ContentType = "text/plain"
                }
            ]
        };

        message.From!.Address.Should().Be("from@example.com");
        message.Cc.Should().HaveCount(1);
        message.Bcc.Should().HaveCount(1);
        message.ReplyTo!.Address.Should().Be("reply@example.com");
        message.HtmlBody.Should().Contain("<p>");
        message.Attachments.Should().HaveCount(1);
        message.Attachments![0].FileName.Should().Be("file.txt");
    }
}

using System.Reflection;
using BuildingBlocks.Application.Email;
using BuildingBlocks.Infrastructure.Email;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Email;

public class SmtpEmailSenderTests
{
    private readonly SmtpOptions _options = new()
    {
        Host = "smtp.example.com",
        Port = 587,
        SenderAddress = "default@example.com",
        SenderName = "Default Sender"
    };

    [Fact]
    public void CreateMailMessage_BasicMessage_SetsFromAndTo()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com", "Recipient")],
            Subject = "Test"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.From!.Address.Should().Be("default@example.com");
        mail.From.DisplayName.Should().Be("Default Sender");
        mail.To.Should().HaveCount(1);
        mail.To[0].Address.Should().Be("to@example.com");
        mail.To[0].DisplayName.Should().Be("Recipient");
        mail.Subject.Should().Be("Test");
    }

    [Fact]
    public void CreateMailMessage_WithFromOverride_UsesOverride()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            From = new EmailAddress("custom@example.com", "Custom"),
            To = [new EmailAddress("to@example.com")],
            Subject = "Test"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.From!.Address.Should().Be("custom@example.com");
        mail.From.DisplayName.Should().Be("Custom");
    }

    [Fact]
    public void CreateMailMessage_WithHtmlBody_SetsIsBodyHtml()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test",
            HtmlBody = "<p>Hello</p>"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.IsBodyHtml.Should().BeTrue();
        mail.Body.Should().Be("<p>Hello</p>");
    }

    [Fact]
    public void CreateMailMessage_WithTextBodyOnly_SetsPlainBody()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test",
            TextBody = "Hello"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.IsBodyHtml.Should().BeFalse();
        mail.Body.Should().Be("Hello");
    }

    [Fact]
    public void CreateMailMessage_WithBothBodies_SetsHtmlAsMainAndPlainAsAlternate()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.IsBodyHtml.Should().BeTrue();
        mail.Body.Should().Be("<p>Hello</p>");
        mail.AlternateViews.Should().HaveCount(1);
    }

    [Fact]
    public void CreateMailMessage_WithCcAndBcc_AddsThem()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Cc = [new EmailAddress("cc@example.com")],
            Bcc = [new EmailAddress("bcc@example.com")],
            Subject = "Test"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.CC.Should().HaveCount(1);
        mail.CC[0].Address.Should().Be("cc@example.com");
        mail.Bcc.Should().HaveCount(1);
        mail.Bcc[0].Address.Should().Be("bcc@example.com");
    }

    [Fact]
    public void CreateMailMessage_WithReplyTo_SetsReplyToList()
    {
        var sender = new SmtpEmailSender(_options);
        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            ReplyTo = new EmailAddress("reply@example.com"),
            Subject = "Test"
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.ReplyToList.Should().HaveCount(1);
        mail.ReplyToList[0].Address.Should().Be("reply@example.com");
    }

    [Fact]
    public void CreateMailMessage_WithAttachment_AddsAttachment()
    {
        var sender = new SmtpEmailSender(_options);
        using var stream = new MemoryStream([1, 2, 3]);

        var message = new EmailMessage
        {
            To = [new EmailAddress("to@example.com")],
            Subject = "Test",
            Attachments =
            [
                new EmailAttachment
                {
                    FileName = "data.bin",
                    Content = stream,
                    ContentType = "application/octet-stream"
                }
            ]
        };

        using var mail = InvokeCreateMailMessage(sender, message);

        mail.Attachments.Should().HaveCount(1);
        mail.Attachments[0].Name.Should().Be("data.bin");
    }

    private static System.Net.Mail.MailMessage InvokeCreateMailMessage(
        SmtpEmailSender sender, EmailMessage message)
    {
        var method = typeof(SmtpEmailSender).GetMethod(
            "CreateMailMessage",
            BindingFlags.NonPublic | BindingFlags.Instance);

        return (System.Net.Mail.MailMessage)method!.Invoke(sender, [message])!;
    }
}

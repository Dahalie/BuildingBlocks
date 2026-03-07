using System.Net;
using System.Net.Mail;
using BuildingBlocks.Application.Email;

namespace BuildingBlocks.Infrastructure.Email;

internal sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(SmtpOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        using var smtpClient = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(message);

        await smtpClient.SendMailAsync(mailMessage, ct);
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl
        };

        if (!string.IsNullOrEmpty(_options.Username))
        {
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        return client;
    }

    private MailMessage CreateMailMessage(EmailMessage message)
    {
        var from = message.From is not null
            ? ToMailAddress(message.From)
            : new MailAddress(_options.SenderAddress, _options.SenderName);

        var mailMessage = new MailMessage
        {
            From = from,
            Subject = message.Subject,
            IsBodyHtml = message.HtmlBody is not null
        };

        if (message.HtmlBody is not null)
        {
            mailMessage.Body = message.HtmlBody;

            if (message.TextBody is not null)
            {
                var plainView = AlternateView.CreateAlternateViewFromString(
                    message.TextBody, null, "text/plain");
                mailMessage.AlternateViews.Add(plainView);
            }
        }
        else
        {
            mailMessage.Body = message.TextBody;
        }

        foreach (var to in message.To)
            mailMessage.To.Add(ToMailAddress(to));

        if (message.Cc is not null)
            foreach (var cc in message.Cc)
                mailMessage.CC.Add(ToMailAddress(cc));

        if (message.Bcc is not null)
            foreach (var bcc in message.Bcc)
                mailMessage.Bcc.Add(ToMailAddress(bcc));

        if (message.ReplyTo is not null)
            mailMessage.ReplyToList.Add(ToMailAddress(message.ReplyTo));

        if (message.Attachments is not null)
            foreach (var attachment in message.Attachments)
                mailMessage.Attachments.Add(new Attachment(attachment.Content, attachment.FileName, attachment.ContentType));

        return mailMessage;
    }

    private static MailAddress ToMailAddress(EmailAddress address) =>
        address.Name is not null
            ? new MailAddress(address.Address, address.Name)
            : new MailAddress(address.Address);
}

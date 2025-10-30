using ECommerce.Core.Common;
using ECommerce.Core.Common.Options;
using ECommerce.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ECommerce.Application.Services;

public class EmailSender(IOptions<EmailOptions> emailOptions) : IEmailSender
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task SendEmailAsync(Email email)
    {
        var mimeMessage = PrepareMimeMessage(email);
        await SendMimeMessageAsync(mimeMessage);
    }

    private MimeMessage PrepareMimeMessage(Email email)
    {
        var mimeMessage = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_emailOptions.SenderEmail),
            Subject = email.Subject
        };

        mimeMessage.From.Add(new MailboxAddress(_emailOptions.DisplayName, _emailOptions.SenderEmail));

        foreach (var recipient in email.To)
            mimeMessage.To.Add(MailboxAddress.Parse(recipient));

        var body = new BodyBuilder();
        if (email.IsHtml) body.HtmlBody = email.Body;
        else body.TextBody = email.Body;
        mimeMessage.Body = body.ToMessageBody();

        if (email.Attachments?.Count > 0)
        {
            ValidateEmailAttachment(email.Attachments);

            byte[] bytes;
            foreach (var file in email.Attachments)
            {
                if (file.Length == 0) continue;

                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();

                body.Attachments.Add(file.FileName, bytes, ContentType.Parse(file.ContentType));
            }
        }

        return mimeMessage;
    }

    private async Task SendMimeMessageAsync(MimeMessage mimeMessage)
    {
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_emailOptions.SmtpHost, _emailOptions.SmtpPort, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.Password);
        await smtpClient.SendAsync(mimeMessage);
        await smtpClient.DisconnectAsync(true);
    }

    private void ValidateEmailAttachment(List<IFormFile> attachments)
    {
        var options = _emailOptions.EmailAttachmentOptions;

        if (attachments.Count > options.MaxFileCount)
            throw new InvalidOperationException($"You can upload up to {options.MaxFileCount} files only.");

        foreach (var file in attachments)
        {
            if (file.Length > options.MaxFileSize)
                throw new InvalidOperationException(
                    $"File {file.FileName} exceeds the maximum allowed size of {options.MaxFileSize / (1024 * 1024)} MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!options.AllowedExtensions.Contains(extension))
                throw new InvalidOperationException($"File type {extension} is not allowed.");
        }
    }
}
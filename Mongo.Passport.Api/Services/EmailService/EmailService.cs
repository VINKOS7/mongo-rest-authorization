using Mongo.Passport.Api.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Mongo.Passport.Api.Serviceses;

public class EmailService : IEmailService
{
    EmailOptions _emailOptions;
    ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailOptions> emailOptions, 
        ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string nick, string email, string subject, string message)
    {
        try
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ChessWood.Support", _emailOptions.Email));
            emailMessage.To.Add(new MailboxAddress(nick, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, true);

                await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Password);

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with attemping send message to email. Ex: {ex}");

            return false;
        }
    }
}

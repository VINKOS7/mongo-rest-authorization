namespace Mongo.Passport.Api.Serviceses;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(string nick, string email, string subject, string message);
}

namespace Mongo.Passport.Infrastructure.HttpClients.YandexCaptha;

public interface IYandexCaptchaHttpClient
{
    public Task<bool> IsRealCaptcha(string captcha, string ip);
}

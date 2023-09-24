using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Mongo.Passport.Infrastructure.HttpClients.YandexCaptha;
using Mongo.Passport.Infrastructure.Options.HttpClientYandexCaptchaOptions;




namespace Mongo.Passport.Infrastructure.HttpClients.YandexCaptcha;

public class YandexCaptchaHttpClient : IYandexCaptchaHttpClient
{
    private readonly HttpClient _httpClient = new();
    private readonly HttpClientYandexCaptchaOptions _httpClientYandexCaptchaOptions;
    private readonly ILogger<YandexCaptchaHttpClient> _logger;

    public YandexCaptchaHttpClient(
        IOptions<HttpClientYandexCaptchaOptions> httpClientYandexCaptchaOptions,
        ILogger<YandexCaptchaHttpClient> logger)
    {
        _httpClientYandexCaptchaOptions = httpClientYandexCaptchaOptions.Value;
        _logger = logger;
    }

    public async Task<bool> IsRealCaptcha(string captcha, string ip)
    {
        try
        {
            var url = $"https://captcha-api.yandex.ru/validate?" +
                $"secret=${_httpClientYandexCaptchaOptions.SecretKey}" +
                $"&token=${captcha}" +
                $"&ip=${ip}";

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode is not HttpStatusCode.OK) return false;

            var responseObj = JsonConvert.DeserializeObject<Captcha>(await response.Content.ReadAsStringAsync());

            if(responseObj is null) return false;

            if(responseObj.Status is not "ok") return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with attemping checking captcha. Ex:{ex}");

            return false;
        }

    }

    private record Captcha([JsonProperty("status")]string Status);
}

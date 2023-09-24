using Mongo.Passport.Domain.Aggregates.Account;
using Mongo.Passport.Infrastructure.HttpClients.YandexCaptcha;
using Mongo.Passport.Infrastructure.HttpClients.YandexCaptha;
using Mongo.Passport.Infrastructure.Repo;

namespace Mongo.Passport.Api.Extensions; 

public static class InfrastructureExtension
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepo, AccountRepo>();
        services.AddScoped<IYandexCaptchaHttpClient, YandexCaptchaHttpClient>();

        return services;
    }
}

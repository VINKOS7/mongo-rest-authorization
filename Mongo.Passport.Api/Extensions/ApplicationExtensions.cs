using Mongo.Passport.Api.Services.SignOutTokenService;
using Mongo.Passport.Api.Serviceses;
using Dotseed.IntegrationEventLog;
using Dotseed.IntegrationEventLog.InMemory;

namespace Mongo.Passport.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();

        //services.AddHostedService<SignOutTokenService>();

        return services;
    }
}

using MongoDB.Driver;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;

using Mongo.Passport.Infrastructure;
using Microsoft.Extensions.Options;

namespace Mongo.Passport.Api.Extensions;

public static class MongoDriverExtensions
{
    public static IServiceCollection ConfigureMongoDriver(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Context");

        services
            .AddSingleton<IMongoClient>(new MongoClient(connectionString))
            .AddSingleton<IContext, Context>();

        return services;
    }
}

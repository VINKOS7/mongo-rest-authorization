using MongoDB.Driver;

using Mongo.Passport.Domain.Aggregates.Account;
using Dotseed.Context;

namespace Mongo.Passport.Infrastructure;

public class Context : IContext//, UnitOfWorkContext
{
    public const string DataBaseName = "ChessWoodPassport";

    public IMongoCollection<Account> Accounts { get; set; }

    public Context(IMongoClient mongoClient)
    {
        IMongoDatabase database = mongoClient.GetDatabase(DataBaseName);

        Accounts = database.GetCollection<Account>("Accounts");
    }
}

using Calabonga.UnitOfWork.MongoDb;
using Mongo.Passport.Domain.Aggregates.Account;
using MongoDB.Driver;

namespace Mongo.Passport.Infrastructure;

public interface IContext //: IUnitOfWork
{
    public IMongoCollection<Account> Accounts { get; set; }
}

using MongoDB.Bson;
//using Calabonga.UnitOfWork.MongoDb;

using Mongo.Passport.Domain.Aggregates.Account.Enums;
using Mongo.Passport.Domain.Aggregates.Account.Values;

namespace Mongo.Passport.Domain.Aggregates.Account;

public interface IAccountRepo //: IRepository
{
    public Task AddAsync(Account account);

    public Task AddDeviceAsync(Account account, Device device);

    public Task RemoveDeviceByTokenAsync(string token);

    public Task SaveChangesAsync(Account accountOld, Account accountNew);

    public Task ChangeActivationCodeAsync(ObjectId AccId, string code);

    public Task ChangePasswordAsync(ObjectId AccId, string password);

    public Task ChangeOnlineAtAsync(ObjectId AccId, DateTime OnlineAt);

    public Task ChangeAccessStatusAsync(ObjectId AccId, AccessStatus accessStatus);

    public void Remove(ObjectId Id);

    public Task<Account> FindAsync(ObjectId Id);

    public Task<Account> FindByEmailAsync(string Email);

    public Task<Account> FindByNickNameAsync(string Nick);

    public Task<Account> FindByIdAsync(ObjectId Id);

    public Task<Account> FindByActivationCodeAsync(string Email);
}

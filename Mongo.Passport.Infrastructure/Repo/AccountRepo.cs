using Mongo.Passport.Domain.Aggregates.Account;
using MongoDB.Driver;
using MongoDB.Bson;

using Mongo.Passport.Domain.Aggregates.Account.Enums;
using Mongo.Passport.Domain.Aggregates.Account.Values;

namespace Mongo.Passport.Infrastructure.Repo;

public class AccountRepo : IAccountRepo//, //IRepository<Account>
{ 
    private readonly IContext _db;

    public AccountRepo(IContext db) => _db = db;
    

    //public IUnitOfWork UnitOfWork => _db;

    public async Task AddAsync(Account account) => await _db.Accounts.InsertOneAsync(account);
    

    public void Remove(ObjectId Id) => _db.Accounts.FindOneAndDelete(acc => acc.Id == Id);
    

    public async Task<Account> FindAsync(ObjectId Id)
    {
        var resultQuery = await _db.Accounts.FindAsync(acc => acc.Id == Id);

        return await resultQuery.FirstOrDefaultAsync();
    }

    public async Task<Account> FindByEmailAsync(string Email)
    {
        var resultQuery = await _db.Accounts.FindAsync(acc => acc.Email == Email);

        return await resultQuery.FirstOrDefaultAsync();
    }

    public async Task<Account> FindByActivationCodeAsync(string ActivationCode)
    {
        var resultQuery = await _db.Accounts.FindAsync(acc => acc.ActivationCode == ActivationCode);

        return await resultQuery.FirstOrDefaultAsync();
    }

    public async Task<Account> FindByIdAsync(ObjectId Id)
    {
        var resultQuery = _db.Accounts.FindAsync(acc => acc.Id == Id);

        return await (await resultQuery).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync(Account accountOld, Account accountNew)
    {
        foreach (var prop in accountOld.GetType().GetProperties())
        {
            if (prop.GetValue(accountOld) != accountNew.GetType().GetProperty(prop.Name).GetValue(accountNew))
            {
                var filter = Builders<Account>.Filter
                    .Eq(acc => prop, prop.GetValue(accountOld));

                var update = Builders<Account>.Update
                    .Set(acc => prop, accountNew.GetType().GetProperty(prop.Name).GetValue(accountNew));

                await _db.Accounts.UpdateOneAsync(filter, update);
            }
        }
    }

    public async Task ChangeActivationCodeAsync(ObjectId AccId, string codeNew)
    {
        var update = Builders<Account>.Update
            .Set(acc => acc.ActivationCode, codeNew);

        await _db.Accounts.UpdateOneAsync(acc => acc.Id == AccId, update);
    }

    public async Task ChangeAccessStatusAsync(ObjectId AccId, AccessStatus accessStatus)
    {
        var update = Builders<Account>.Update
            .Set(acc => acc.AccessStatus, accessStatus);

        await _db.Accounts.UpdateOneAsync(acc => acc.Id == AccId, update);
    }

    public async Task ChangePasswordAsync(ObjectId AccId, string password)
    {
        var update = Builders<Account>.Update
            .Set(acc => acc.Password, password);

        await _db.Accounts.UpdateOneAsync(acc => acc.Id == AccId, update);
    }

    public async Task AddDeviceAsync(Account account, Device device)
    {
        account.Devices.Add(device);

        var update = Builders<Account>.Update
            .Set(acc => acc.Devices, account.Devices);

        await _db.Accounts.UpdateOneAsync(acc => acc.Id == account.Id, update);
    }

    public async Task RemoveDeviceByTokenAsync(string token) => await _db.Accounts
        .FindOneAndDeleteAsync(acc => acc.Devices.FirstOrDefault(d => d.Token == token) != null);
    

    public async Task<Account> FindByNickNameAsync(string Nick)
    {
        var resultQuery = _db.Accounts.FindAsync(acc => acc.Nickname == Nick);

        return await (await resultQuery).FirstOrDefaultAsync();
    }

    public Task ChangeOnlineAtAsync(ObjectId AccId, DateTime OnlineAt)
    {
        throw new NotImplementedException();
    }
}

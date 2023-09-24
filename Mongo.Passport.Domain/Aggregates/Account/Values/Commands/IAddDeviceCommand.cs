using MongoDB.Bson;

namespace Mongo.Passport.Domain.Aggregates.Account.Values.Commands;

public interface IAddDeviceCommand
{
    public ObjectId Id { get; set; }
    public string Name { get; }
    public string Version { get; }
    public string Token { get; }
    public DateTime OnlineAt { get; }
    public DateTime CreatedAt { get; }
}

using Dotseed.Domain;
using MongoDB.Bson;

using Mongo.Passport.Domain.Aggregates.Account.Values.Commands;

namespace Mongo.Passport.Domain.Aggregates.Account.Values;

public class Device : ValueObject
{
    public Device From(IAddDeviceCommand command)
    {
        return new()
        {
            Id = command.Id, 
            Name = command.Name,
            Version = command.Version,
            Token = command.Token,
            OnlineAt = command.OnlineAt,
            CreatedAt = command.CreatedAt,
        };
    }

    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string Token { get; set; }
    public DateTime OnlineAt { get; set; }
    public DateTime CreatedAt { get; set; }


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Version;
        yield return Token;
        yield return OnlineAt;
        yield return CreatedAt;
    }
}

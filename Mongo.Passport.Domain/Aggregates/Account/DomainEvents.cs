using MediatR;
using MongoDB.Bson;

namespace Mongo.Passport.Domain.Aggregates.Account;

public record AddAccountDomainEvent (
    ObjectId UserId,
    string Firstname,
    string Lastname
) 
: INotification;
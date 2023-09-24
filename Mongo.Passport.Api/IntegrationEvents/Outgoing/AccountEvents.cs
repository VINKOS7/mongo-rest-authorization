using MongoDB.Bson;

using Dotseed.IntegrationEventLog;

namespace Mongo.Passport.Api.IntegrationEvents.Outgoing;

public class AddAccountIntegrationEvent : IntegrationEvent
{
    public AddAccountIntegrationEvent(ObjectId _dialogUserId)
    {
        DialogUserId = _dialogUserId.ToString();
    }

    public string DialogUserId { get; }
}
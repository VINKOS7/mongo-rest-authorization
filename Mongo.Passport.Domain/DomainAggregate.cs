using MediatR;
using MongoDB.Bson;

namespace Mongo.Passport.Domain;

public abstract class DomainAggregate
{
    public ObjectId Id { get; init; } = ObjectId.GenerateNewId();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private List<INotification> _domainEvents;

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void PublishDomainEvents(IMediator _mediator) 
    {
        _domainEvents.ForEach(e => _mediator.Publish(e));

        _domainEvents.Clear();
    }

    public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents?.Clear();

    public bool IsTransient() => Id == default;
    
}

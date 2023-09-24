using Dotseed.IntegrationEventLog;
using MediatR;

using Mongo.Passport.Domain.Aggregates.Account;
using Mongo.Passport.Api.IntegrationEvents.Outgoing;

namespace Mongo.Passport.Api.DomainEventHandlers.AccountHandlers;

public class AddAccountDomainEventHandler //: INotificationHandler<AddAccountDomainEvent>//If you want use RabbitMQ
{
    private readonly IIntegrationEventLogService _integrationEventLogService;

    public AddAccountDomainEventHandler(IIntegrationEventLogService integrationEventLogService) => _integrationEventLogService = integrationEventLogService;

    public async Task Handle(AddAccountDomainEvent notification, CancellationToken cancellationToken)
    {
        _integrationEventLogService.StoreEventForDelayedPublishing(new AddAccountIntegrationEvent(notification.UserId));

        await _integrationEventLogService.PublishStoredIntegrationEventsAsync();
    }
}

using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Authentication.Domain.Events;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.Authentication.Application.EventHandlers;

public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(IPublishEndpoint publishEndpoint, ILogger<UserCreatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing UserCreatedIntegrationEvent for user: {UserId}", notification.UserId);
        
        var integrationEvent = new UserCreatedIntegrationEvent(
            notification.UserId,
            notification.Email,
            "Unknown", // We don't have first/last name in domain event
            "Unknown", 
            notification.UserType.ToString(),
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published UserCreatedIntegrationEvent for user: {UserId}", notification.UserId);
    }
}
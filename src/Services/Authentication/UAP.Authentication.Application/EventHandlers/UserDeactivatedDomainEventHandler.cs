using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Authentication.Domain.Events;
using UAP.Authentication.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.Authentication.Application.EventHandlers;

public class UserDeactivatedDomainEventHandler : INotificationHandler<UserDeactivatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserDeactivatedDomainEventHandler> _logger;
    private readonly IUserRepository _userRepository;

    public UserDeactivatedDomainEventHandler(
        IPublishEndpoint publishEndpoint, 
        ILogger<UserDeactivatedDomainEventHandler> logger,
        IUserRepository userRepository)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Handle(UserDeactivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing UserDeactivatedIntegrationEvent for user: {UserId}", notification.UserId);

        try
        {
            // Get user details for the integration event
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found for integration event: {UserId}", notification.UserId);
                return;
            }

            var integrationEvent = new UserDeactivatedIntegrationEvent(
                user.Id,
                user.Email,
                user.UserType.ToString(),
                notification.DeactivatedAt
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published UserDeactivatedIntegrationEvent for user: {UserId}", notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing UserDeactivatedIntegrationEvent for user: {UserId}", notification.UserId);
            throw;
        }
    }
}
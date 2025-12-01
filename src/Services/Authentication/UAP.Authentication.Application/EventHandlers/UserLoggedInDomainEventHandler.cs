using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Authentication.Domain.Events;
using UAP.Authentication.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.Authentication.Application.EventHandlers;
/// <summary>
/// Handles UserLoggedInDomainEvent and publishes corresponding integration event
/// This allows other services to react to user login activities
/// </summary>
public class UserLoggedInDomainEventHandler : INotificationHandler<UserLoggedInDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UserLoggedInDomainEventHandler> _logger;
    private readonly IUserRepository _userRepository;

    public UserLoggedInDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<UserLoggedInDomainEventHandler> logger,
        IUserRepository userRepository)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Handle(UserLoggedInDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing UserLoggedInIntegrationEvent for user: {UserId} - {Email}",
            notification.UserId, notification.Email);

        try
        {
            // Get additional user details for the integration event
            var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found for login event: {UserId}", notification.UserId);
                return;
            }

            var integrationEvent = new UserLoggedInIntegrationEvent(
                user.Id,
                user.Email,
                DateTime.UtcNow // Use current time for integration event
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published UserLoggedInIntegrationEvent for user: {UserId}", notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing UserLoggedInIntegrationEvent for user: {UserId}", notification.UserId);
            throw;
        }
    }
}
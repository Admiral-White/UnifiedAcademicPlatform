using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class CourseFullDomainEventHandler : INotificationHandler<CourseFullDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CourseFullDomainEventHandler> _logger;

    public CourseFullDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CourseFullDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(CourseFullDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CourseFullIntegrationEvent for course: {CourseCode} - {Title}",
            notification.CourseCode, notification.Title);

        var integrationEvent = new CourseFullIntegrationEvent(
            notification.CourseId,
            notification.CourseCode,
            notification.Title,
            0, // MaxCapacity would need to be loaded from DB
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published CourseFullIntegrationEvent for course: {CourseId}", notification.CourseId);
    }
}
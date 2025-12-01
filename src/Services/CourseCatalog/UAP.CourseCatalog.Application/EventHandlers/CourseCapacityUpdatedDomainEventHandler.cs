using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class CourseCapacityUpdatedDomainEventHandler : INotificationHandler<CourseCapacityUpdatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CourseCapacityUpdatedDomainEventHandler> _logger;

    public CourseCapacityUpdatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CourseCapacityUpdatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(CourseCapacityUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CourseCapacityUpdatedIntegrationEvent for course: {CourseCode} - Capacity: {MaxCapacity}, Enrollment: {CurrentEnrollment}",
            notification.CourseCode, notification.MaxCapacity, notification.CurrentEnrollment);

        var integrationEvent = new CourseCapacityUpdatedIntegrationEvent(
            notification.CourseId,
            notification.CourseCode,
            notification.MaxCapacity,
            notification.CurrentEnrollment,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published CourseCapacityUpdatedIntegrationEvent for course: {CourseId}", notification.CourseId);
    }
}
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.Shared.Contracts.IntegrationEvents;
namespace UAP.CourseCatalog.Application.EventHandlers;

public class CourseCreatedDomainEventHandler : INotificationHandler<CourseCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CourseCreatedDomainEventHandler> _logger;

    public CourseCreatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CourseCreatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(CourseCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CourseCreatedIntegrationEvent for course: {CourseCode} - {Title}",
            notification.CourseCode, notification.Title);

        var integrationEvent = new CourseCreatedIntegrationEvent(
            notification.CourseId,
            notification.CourseCode,
            notification.Title,
            "Description not available in domain event", // Would need to load from DB in real scenario
            0, // Credits not available
            notification.DepartmentId,
            Guid.Empty, // CoordinatorId not available
            0, // MaxCapacity not available
            false, // IsBorrowable not available
            "Fall", // Semester not available
            DateTime.UtcNow.Year, // AcademicYear not available
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published CourseCreatedIntegrationEvent for course: {CourseId}", notification.CourseId);
    }
    
}
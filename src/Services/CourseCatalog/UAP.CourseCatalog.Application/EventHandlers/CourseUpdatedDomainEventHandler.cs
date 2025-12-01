using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class CourseUpdatedDomainEventHandler : INotificationHandler<CourseUpdatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CourseUpdatedDomainEventHandler> _logger;
    private readonly ICourseRepository _courseRepository;

    public CourseUpdatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CourseUpdatedDomainEventHandler> logger,
        ICourseRepository courseRepository)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _courseRepository = courseRepository;
    }

    public async Task Handle(CourseUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CourseUpdatedIntegrationEvent for course: {CourseCode} - {Title}",
            notification.CourseCode, notification.Title);

        try
        {
            // Get the full course details to include in the integration event
            var course = await _courseRepository.GetByIdWithDetailsAsync(notification.CourseId, cancellationToken);
            if (course == null)
            {
                _logger.LogWarning("Course not found for integration event: {CourseId}", notification.CourseId);
                return;
            }

            var integrationEvent = new CourseUpdatedIntegrationEvent(
                course.Id,
                course.CourseCode,
                course.Title,
                course.Description,
                course.Credits,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published CourseUpdatedIntegrationEvent for course: {CourseId}", notification.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing CourseUpdatedIntegrationEvent for course: {CourseId}", notification.CourseId);
            throw;
        }
    }
}
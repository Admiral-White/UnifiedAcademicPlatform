using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class CourseDeactivatedDomainEventHandler : INotificationHandler<CourseDeactivatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CourseDeactivatedDomainEventHandler> _logger;
    private readonly ICourseRepository _courseRepository;

    public CourseDeactivatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CourseDeactivatedDomainEventHandler> logger,
        ICourseRepository courseRepository)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _courseRepository = courseRepository;
    }

    public async Task Handle(CourseDeactivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CourseDeactivatedIntegrationEvent for course: {CourseCode} - {Title}",
            notification.CourseCode, notification.Title);

        try
        {
            // Get the full course details
            var course = await _courseRepository.GetByIdAsync(notification.CourseId, cancellationToken);
            if (course == null)
            {
                _logger.LogWarning("Course not found for integration event: {CourseId}", notification.CourseId);
                return;
            }

            var integrationEvent = new CourseDeactivatedIntegrationEvent(
                course.Id,
                course.CourseCode,
                course.Title,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published CourseDeactivatedIntegrationEvent for course: {CourseId}", notification.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing CourseDeactivatedIntegrationEvent for course: {CourseId}", notification.CourseId);
            throw;
        }
    }
}
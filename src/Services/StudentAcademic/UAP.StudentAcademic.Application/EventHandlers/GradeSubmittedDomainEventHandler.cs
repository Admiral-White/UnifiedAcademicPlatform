using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.StudentAcademic.Domain.Events;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class GradeSubmittedDomainEventHandler : INotificationHandler<GradeSubmittedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<GradeSubmittedDomainEventHandler> _logger;

    public GradeSubmittedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<GradeSubmittedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(GradeSubmittedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing GradeSubmittedIntegrationEvent for student: {StudentId}, Course: {CourseId}, Grade: {Grade}",
            notification.StudentId, notification.CourseId, notification.Grade);

        try
        {
            var integrationEvent = new GradeSubmittedIntegrationEvent(
                notification.StudentId,
                notification.CourseId,
                notification.Grade,
                notification.Semester,
                notification.AcademicYear,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published GradeSubmittedIntegrationEvent for student: {StudentId}", notification.StudentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing GradeSubmittedIntegrationEvent for student: {StudentId}", notification.StudentId);
            throw;
        }
    }
}

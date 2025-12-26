using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.StudentAcademic.Domain.Events;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class StudentStatusChangedDomainEventHandler : INotificationHandler<StudentStatusChangedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<StudentStatusChangedDomainEventHandler> _logger;

    public StudentStatusChangedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<StudentStatusChangedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(StudentStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing StudentStatusChangedIntegrationEvent for student: {StudentId}, Old: {OldStatus}, New: {NewStatus}",
            notification.StudentId, notification.OldStatus, notification.NewStatus);

        try
        {
            var integrationEvent = new StudentStatusChangedIntegrationEvent(
                notification.StudentId,
                notification.OldStatus,
                notification.NewStatus,
                notification.Reason,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published StudentStatusChangedIntegrationEvent for student: {StudentId}", notification.StudentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing StudentStatusChangedIntegrationEvent for student: {StudentId}", notification.StudentId);
            throw;
        }
    }
}

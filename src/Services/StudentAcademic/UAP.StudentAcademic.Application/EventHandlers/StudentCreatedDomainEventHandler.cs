using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.StudentAcademic.Domain.Events;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class StudentCreatedDomainEventHandler : INotificationHandler<StudentCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<StudentCreatedDomainEventHandler> _logger;

    public StudentCreatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<StudentCreatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(StudentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing StudentCreatedIntegrationEvent for student: {StudentNumber}",
            notification.StudentNumber);

        try
        {
            var integrationEvent = new StudentCreatedIntegrationEvent(
                notification.StudentId,
                notification.StudentNumber,
                notification.UserId,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published StudentCreatedIntegrationEvent for student: {StudentId}", notification.StudentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing StudentCreatedIntegrationEvent for student: {StudentId}", notification.StudentId);
            throw;
        }
    }
}
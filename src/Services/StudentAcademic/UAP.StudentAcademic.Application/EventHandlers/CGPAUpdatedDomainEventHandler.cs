using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.StudentAcademic.Domain.Events;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class CGPAUpdatedDomainEventHandler : INotificationHandler<CGPAUpdatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CGPAUpdatedDomainEventHandler> _logger;

    public CGPAUpdatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CGPAUpdatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(CGPAUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing CGPAUpdatedIntegrationEvent for student: {StudentId}, Old: {OldCGPA}, New: {NewCGPA}",
            notification.StudentId, notification.OldCGPA, notification.NewCGPA);

        try
        {
            var integrationEvent = new CGPAUpdatedIntegrationEvent(
                notification.StudentId,
                notification.OldCGPA,
                notification.NewCGPA,
                DateTime.UtcNow
            );

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            _logger.LogInformation("Successfully published CGPAUpdatedIntegrationEvent for student: {StudentId}", notification.StudentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing CGPAUpdatedIntegrationEvent for student: {StudentId}", notification.StudentId);
            throw;
        }
    }
}
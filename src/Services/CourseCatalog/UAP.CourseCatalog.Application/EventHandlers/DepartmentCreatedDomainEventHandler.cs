using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Events;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class DepartmentCreatedDomainEventHandler : INotificationHandler<DepartmentCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DepartmentCreatedDomainEventHandler> _logger;

    public DepartmentCreatedDomainEventHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<DepartmentCreatedDomainEventHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(DepartmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing DepartmentCreatedIntegrationEvent for department: {Code} - {Name}",
            notification.Code, notification.Name);

        var integrationEvent = new DepartmentCreatedIntegrationEvent(
            notification.DepartmentId,
            notification.Code,
            notification.Name,
            "Description not available in domain event", // Would need to load from DB
            Guid.Empty, // HeadOfDepartmentId not available
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published DepartmentCreatedIntegrationEvent for department: {DepartmentId}", notification.DepartmentId);
    }
}
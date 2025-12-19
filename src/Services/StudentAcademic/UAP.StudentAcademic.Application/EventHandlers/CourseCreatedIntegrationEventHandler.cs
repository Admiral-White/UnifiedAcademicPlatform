using MassTransit;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class CourseCreatedIntegrationEventHandler : IConsumer<CourseCreatedIntegrationEvent>
{
    private readonly ILogger<CourseCreatedIntegrationEventHandler> _logger;

    public CourseCreatedIntegrationEventHandler(ILogger<CourseCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CourseCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Received CourseCreatedIntegrationEvent: {CourseCode} - {Title}",
            message.CourseCode, message.Title);

        try
        {
            // In Student Academic Service, we might want to:
            // 1. Cache course information for quick access
            // 2. Update course references in student records
            // 3. Prepare course analytics data
            
            _logger.LogDebug("Course information cached for academic tracking: {CourseId}", message.CourseId);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CourseCreatedIntegrationEvent: {CourseId}", message.CourseId);
            throw;
        }
    }
}
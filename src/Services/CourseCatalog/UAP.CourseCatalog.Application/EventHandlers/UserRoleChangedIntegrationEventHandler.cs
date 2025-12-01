using MassTransit;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class UserRoleChangedIntegrationEventHandler : IConsumer<UserRoleChangedIntegrationEvent>
{
    private readonly ILogger<UserRoleChangedIntegrationEventHandler> _logger;
    private readonly ICourseRepository _courseRepository;

    public UserRoleChangedIntegrationEventHandler(
        ILogger<UserRoleChangedIntegrationEventHandler> logger,
        ICourseRepository courseRepository)
    {
        _logger = logger;
        _courseRepository = courseRepository;
    }

    public async Task Consume(ConsumeContext<UserRoleChangedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Received UserRoleChangedIntegrationEvent for user: {UserId}, From: {PreviousRole} To: {NewRole}",
            message.UserId, message.PreviousRole, message.NewRole);

        try
        {
            // Handle role-specific changes
            if (message.PreviousRole == "CourseCoordinator" && message.NewRole != "CourseCoordinator")
            {
                // User is no longer a coordinator, need to reassign courses
                await HandleCoordinatorRoleRemoval(message.UserId);
            }
            else if (message.PreviousRole != "CourseCoordinator" && message.NewRole == "CourseCoordinator")
            {
                // User became a coordinator, might need to initialize coordinator data
                await HandleNewCoordinator(message.UserId);
            }

            _logger.LogInformation("Successfully processed UserRoleChangedIntegrationEvent for user: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserRoleChangedIntegrationEvent for user: {UserId}", message.UserId);
            throw;
        }
    }

    private async Task HandleCoordinatorRoleRemoval(Guid userId)
    {
        _logger.LogInformation("User {UserId} is no longer a coordinator. Reassigning courses.", userId);
        
        var courses = await _courseRepository.GetByCoordinatorAsync(userId);
        if (courses.Any())
        {
            _logger.LogInformation("Found {CourseCount} courses to reassign from coordinator {UserId}", courses.Count, userId);
            
            // In a real implementation, we would reassign to department head or admin
            // For now, we'll just log the action
            foreach (var course in courses)
            {
                _logger.LogWarning("Course {CourseCode} needs reassignment from coordinator {UserId}", course.CourseCode, userId);
            }
        }
    }

    private async Task HandleNewCoordinator(Guid userId)
    {
        _logger.LogInformation("User {UserId} is now a coordinator. Initializing coordinator data.", userId);
        
        // In a real implementation, we might:
        // - Initialize coordinator preferences
        // - Set up default course templates
        // - Assign to a department if not already assigned
        
        await Task.CompletedTask;
    }
}
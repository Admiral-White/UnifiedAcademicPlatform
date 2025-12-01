using MassTransit;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.CourseCatalog.Application.EventHandlers;

public class UserDeactivatedIntegrationEventHandler : IConsumer<UserLoggedInIntegrationEvent.UserDeactivatedIntegrationEvent>
{
    private readonly ILogger<UserDeactivatedIntegrationEventHandler> _logger;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserDeactivatedIntegrationEventHandler(
        ILogger<UserDeactivatedIntegrationEventHandler> logger,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserLoggedInIntegrationEvent.UserDeactivatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Received UserDeactivatedIntegrationEvent for user: {UserId}, Email: {Email}",
            message.UserId, message.Email);

        try
        {
            // When a user is deactivated, we need to handle course coordinator reassignment
            await HandleCoordinatorReassignment(message.UserId);

            _logger.LogInformation("Successfully processed UserDeactivatedIntegrationEvent for user: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserDeactivatedIntegrationEvent for user: {UserId}", message.UserId);
            throw;
        }
    }

    private async Task HandleCoordinatorReassignment(Guid userId)
    {
        // Find all courses where this user is the coordinator
        var courses = await _courseRepository.GetByCoordinatorAsync(userId);
        
        if (courses.Any())
        {
            _logger.LogInformation(
                "Found {CourseCount} courses coordinated by user {UserId}. Reassigning to default coordinator.",
                courses.Count, userId);

            // In a real scenario, we would:
            // 1. Find a replacement coordinator (perhaps department head or admin)
            // 2. Update all courses to use the new coordinator
            // 3. Notify relevant stakeholders about the change
            
            // For now, we'll set a default system coordinator
            var defaultCoordinatorId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            
            foreach (var course in courses)
            {
                // Use reflection to update the coordinator ID since it's private
                // In a real implementation, we'd have a proper method to update coordinator
                var coordinatorProperty = typeof(Course).GetProperty("CoordinatorId");
                if (coordinatorProperty != null && coordinatorProperty.CanWrite)
                {
                    coordinatorProperty.SetValue(course, defaultCoordinatorId);
                    await _courseRepository.UpdateAsync(course);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation(
                "Successfully reassigned {CourseCount} courses from coordinator {UserId} to default coordinator",
                courses.Count, userId);
        }
        else
        {
            _logger.LogInformation("No courses found coordinated by user {UserId}", userId);
        }
    }
}
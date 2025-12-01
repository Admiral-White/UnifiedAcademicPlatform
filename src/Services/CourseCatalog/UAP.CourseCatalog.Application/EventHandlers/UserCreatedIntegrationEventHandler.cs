using MassTransit;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Contracts.IntegrationEvents;

namespace UAP.CourseCatalog.Application.EventHandlers;

/// <summary>
/// Handles UserCreatedIntegrationEvent from Authentication service
/// This handler processes new user creation events to perform course catalog specific actions
/// </summary>
public class UserCreatedIntegrationEventHandler : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;
    private readonly ICourseRepository _courseRepository;

    public UserCreatedIntegrationEventHandler(
        ILogger<UserCreatedIntegrationEventHandler> logger,
        ICourseRepository courseRepository)
    {
        _logger = logger;
        _courseRepository = courseRepository;
    }

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Received UserCreatedIntegrationEvent for user: {UserId}, Email: {Email}, Type: {UserType}",
            message.UserId, message.Email, message.UserType);

        try
        {
            // Handle different user types
            switch (message.UserType.ToLower())
            {
                case "coursecoordinator":
                    await HandleCourseCoordinatorCreation(message);
                    break;
                case "student":
                    await HandleStudentCreation(message);
                    break;
                case "staff":
                case "administrator":
                    await HandleStaffCreation(message);
                    break;
                default:
                    _logger.LogWarning("Unknown user type: {UserType} for user: {UserId}", message.UserType, message.UserId);
                    break;
            }

            _logger.LogInformation("Successfully processed UserCreatedIntegrationEvent for user: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserCreatedIntegrationEvent for user: {UserId}", message.UserId);
            
            // In a production scenario, you might want to implement retry logic or dead-letter queue handling
            throw;
        }
    }

    private async Task HandleCourseCoordinatorCreation(UserCreatedIntegrationEvent message)
    {
        _logger.LogInformation(
            "Processing course coordinator creation: {UserId} - {Email}",
            message.UserId, message.Email);

        // When a new course coordinator is created, we might want to:
        // 1. Cache coordinator information for quick access
        // 2. Update any existing courses that might be assigned to this coordinator
        // 3. Send welcome notification specific to coordinators
        // 4. Initialize default coordinator settings

        // For now, we'll just log the action
        // In a real implementation, you might:
        // - Update a coordinator cache
        // - Initialize coordinator-specific data
        // - Send internal notifications

        await Task.CompletedTask;
    }

    private async Task HandleStudentCreation(UserCreatedIntegrationEvent message)
    {
        _logger.LogInformation(
            "Processing student creation: {UserId} - {Email}",
            message.UserId, message.Email);

        // When a new student is created, we might want to:
        // 1. Cache student information
        // 2. Initialize student-specific course preferences
        // 3. Set up default course recommendations
        // 4. Prepare student dashboard data

        // For now, we'll just log the action
        // In a real implementation, you might:
        // - Update student cache
        // - Initialize student academic profile
        // - Prepare course recommendations based on department

        await Task.CompletedTask;
    }

    private async Task HandleStaffCreation(UserCreatedIntegrationEvent message)
    {
        _logger.LogInformation(
            "Processing staff creation: {UserId} - {Email}",
            message.UserId, message.Email);

        // When a new staff member is created, we might want to:
        // 1. Cache staff information
        // 2. Set up department associations if applicable
        // 3. Initialize staff-specific permissions for course management

        // For now, we'll just log the action
        // In a real implementation, you might:
        // - Update staff cache
        // - Initialize department associations
        // - Set up course management permissions

        await Task.CompletedTask;
    }
}
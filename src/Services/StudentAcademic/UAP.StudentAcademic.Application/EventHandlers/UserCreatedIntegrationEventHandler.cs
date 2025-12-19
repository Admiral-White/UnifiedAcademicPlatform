using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Shared.Contracts.IntegrationEvents;
using UAP.StudentAcademic.Application.Commands;

namespace UAP.StudentAcademic.Application.EventHandlers;

public class UserCreatedIntegrationEventHandler : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;
    private readonly IMediator _mediator;

    public UserCreatedIntegrationEventHandler(
        ILogger<UserCreatedIntegrationEventHandler> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Received UserCreatedIntegrationEvent for user: {UserId}, Type: {UserType}",
            message.UserId, message.UserType);

        try
        {
            // Only create student record if user type is Student
            if (message.UserType.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                await CreateStudentRecord(message);
            }

            _logger.LogInformation("Successfully processed UserCreatedIntegrationEvent for user: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserCreatedIntegrationEvent for user: {UserId}", message.UserId);
            throw;
        }
    }
    
    private async Task CreateStudentRecord(UserCreatedIntegrationEvent message)
    {
        // Generate student number (format: YYMMDD-XXXX)
        var studentNumber = GenerateStudentNumber();
        
        // For now, use default department and program
        // In real implementation, this would come from user registration data
        var defaultDepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var defaultProgram = "Undergraduate";
        
        var command = new CreateStudentCommand(
            studentNumber,
            message.UserId,
            defaultDepartmentId,
            defaultProgram,
            DateTime.UtcNow
        );

        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Student record created: {StudentId} for user: {UserId}",
                result.Value, message.UserId);
        }
        else
        {
            _logger.LogWarning(
                "Failed to create student record for user: {UserId}, Error: {Error}",
                message.UserId, result.Error);
        }
    }
    
    private string GenerateStudentNumber()
    {
        var now = DateTime.UtcNow;
        var datePart = now.ToString("yyMMdd");
        var randomPart = new Random().Next(1000, 9999).ToString("D4");
        
        return $"{datePart}-{randomPart}";
    }
}
namespace UAP.Shared.Contracts.IntegrationEvents;

public class StudentCreatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public StudentCreatedIntegrationEvent(Guid studentId, string studentNumber, Guid userId, DateTime createdAt)
    {
        StudentId = studentId;
        StudentNumber = studentNumber;
        UserId = userId;
        CreatedAt = createdAt;
    }
}

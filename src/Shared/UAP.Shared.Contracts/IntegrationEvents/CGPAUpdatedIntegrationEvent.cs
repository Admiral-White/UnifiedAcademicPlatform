namespace UAP.Shared.Contracts.IntegrationEvents;

public class CGPAUpdatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid StudentId { get; set; }
    public decimal OldCGPA { get; set; }
    public decimal NewCGPA { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CGPAUpdatedIntegrationEvent(Guid studentId, decimal oldCGPA, decimal newCGPA, DateTime updatedAt)
    {
        StudentId = studentId;
        OldCGPA = oldCGPA;
        NewCGPA = newCGPA;
        UpdatedAt = updatedAt;
    }
}

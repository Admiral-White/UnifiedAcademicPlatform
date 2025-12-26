namespace UAP.Shared.Contracts.IntegrationEvents;

public class StudentStatusChangedIntegrationEvent : BaseIntegrationEvent
{
    public Guid StudentId { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
    public string Reason { get; set; }
    public DateTime ChangedAt { get; set; }

    public StudentStatusChangedIntegrationEvent(Guid studentId, string oldStatus, string newStatus, string reason, DateTime changedAt)
    {
        StudentId = studentId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedAt = changedAt;
    }
}

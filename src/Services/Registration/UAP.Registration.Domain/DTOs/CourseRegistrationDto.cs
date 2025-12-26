using UAP.Registration.Domain.Enums;

namespace UAP.Registration.Domain.DTOs;

public class CourseRegistrationDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string SemesterId { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public RegistrationStatus Status { get; set; }
    public RegistrationType Type { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedBy { get; set; }
}
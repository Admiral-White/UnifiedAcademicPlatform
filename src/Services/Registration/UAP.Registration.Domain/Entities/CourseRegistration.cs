using UAP.Registration.Domain.Enums;
using UAP.Registration.Domain.Events;
using UAP.SharedKernel.Entity;

namespace UAP.Registration.Domain.Entities;

public class CourseRegistration : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string SemesterId { get; private set; }
    public int AcademicYear { get; private set; }
    public RegistrationStatus Status { get; private set; }
    public RegistrationType Type { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CourseRegistration() { }

    public static CourseRegistration Create(
        Guid studentId,
        Guid courseId,
        string semesterId,
        int academicYear,
        RegistrationType type)
    {
        var registration = new CourseRegistration
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            SemesterId = semesterId,
            AcademicYear = academicYear,
            Type = type,
            Status = RegistrationStatus.Pending,
            RegistrationDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        registration.AddDomainEvent(new CourseRegistrationCreatedEvent(registration.Id, studentId, courseId));
        return registration;
    }

    public void Approve(Guid approvedBy)
    {
        Status = RegistrationStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CourseRegistrationApprovedEvent(Id, StudentId, CourseId));
    }

    public void Reject(string reason)
    {
        Status = RegistrationStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CourseRegistrationRejectedEvent(Id, StudentId, CourseId, reason));
    }

    public void Cancel()
    {
        Status = RegistrationStatus.Cancelled;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CourseRegistrationCancelledEvent(Id, StudentId, CourseId));
    }

    public void Complete()
    {
        Status = RegistrationStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}

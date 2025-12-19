using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Domain.Events;

public class StudentCreatedDomainEvent : DomainEvent
{
    public Guid StudentId { get; }
    public string StudentNumber { get; }
    public Guid UserId { get; }

    public StudentCreatedDomainEvent(Guid studentId, string studentNumber, Guid userId)
    {
        StudentId = studentId;
        StudentNumber = studentNumber;
        UserId = userId;
    }
}

public class GradeSubmittedDomainEvent : DomainEvent
{
    public Guid StudentId { get; }
    public Guid CourseId { get; }
    public string Grade { get; }
    public string Semester { get; }
    public int AcademicYear { get; }

    public GradeSubmittedDomainEvent(Guid studentId, Guid courseId, string grade, string semester, int academicYear)
    {
        StudentId = studentId;
        CourseId = courseId;
        Grade = grade;
        Semester = semester;
        AcademicYear = academicYear;
    }
}

public class CGPAUpdatedDomainEvent : DomainEvent
{
    public Guid StudentId { get; }
    public decimal OldCGPA { get; }
    public decimal NewCGPA { get; }

    public CGPAUpdatedDomainEvent(Guid studentId, decimal oldCGPA, decimal newCGPA)
    {
        StudentId = studentId;
        OldCGPA = oldCGPA;
        NewCGPA = newCGPA;
    }
}

public class StudentStatusChangedDomainEvent : DomainEvent
{
    public Guid StudentId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public string Reason { get; }

    public StudentStatusChangedDomainEvent(Guid studentId, string oldStatus, string newStatus, string reason)
    {
        StudentId = studentId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Domain.Events;

public class CourseCapacityUpdatedDomainEvent : DomainEvent
{
    public Guid CourseId { get; }
    public string CourseCode { get; }
    public int MaxCapacity { get; }
    public int CurrentEnrollment { get; }

    public CourseCapacityUpdatedDomainEvent(Guid courseId, string courseCode, int maxCapacity, int currentEnrollment)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        MaxCapacity = maxCapacity;
        CurrentEnrollment = currentEnrollment;
    }
}
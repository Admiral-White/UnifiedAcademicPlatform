using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Domain.Events;

public class CourseDeactivatedDomainEvent : DomainEvent
{
    public Guid CourseId { get; }
    public string CourseCode { get; }
    public string Title { get; }

    public CourseDeactivatedDomainEvent(Guid courseId, string courseCode, string title)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
    }
}
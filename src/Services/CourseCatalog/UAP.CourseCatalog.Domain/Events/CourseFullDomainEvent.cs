using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Domain.Events;

public class CourseFullDomainEvent : DomainEvent
{
    public Guid CourseId { get; }
    public string CourseCode { get; }
    public string Title { get; }

    public CourseFullDomainEvent(Guid courseId, string courseCode, string title)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
    }
}
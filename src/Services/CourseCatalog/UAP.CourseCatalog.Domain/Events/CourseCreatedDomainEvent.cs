using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Domain.Events;

public class CourseCreatedDomainEvent : DomainEvent
{
    public Guid CourseId { get; }
    public string CourseCode { get; }
    public string Title { get; }
    public Guid DepartmentId { get; }

    public CourseCreatedDomainEvent(Guid courseId, string courseCode, string title, Guid departmentId)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
        DepartmentId = departmentId;
    }
}
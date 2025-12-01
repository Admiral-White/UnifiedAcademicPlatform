using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Queries;

public class GetCourseQuery : IRequest<Result<CourseDto>>
{
    public Guid CourseId { get; set; }

    public GetCourseQuery(Guid courseId)
    {
        CourseId = courseId;
    }
}

public class CourseDto
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public Guid CoordinatorId { get; set; }
    public string CoordinatorName { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public bool IsActive { get; set; }
    public bool IsBorrowable { get; set; }
    public string OfferingSemester { get; set; }
    public int AcademicYear { get; set; }
    public List<PrerequisiteDto> Prerequisites { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}

public class PrerequisiteDto
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
}
using MediatR;
using UAP.CourseCatalog.Domain.Enums;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Commands;

public class CreateCourseCommand : IRequest<Result<Guid>>
{
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid CoordinatorId { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsBorrowable { get; set; }
    public Semester OfferingSemester { get; set; }
    public int AcademicYear { get; set; }
    public List<Guid> Prerequisites { get; set; } = new();

    public CreateCourseCommand(
        string courseCode,
        string title,
        string description,
        int credits,
        Guid departmentId,
        Guid coordinatorId,
        int maxCapacity,
        bool isBorrowable,
        Semester offeringSemester,
        int academicYear,
        List<Guid> prerequisites = null)
    {
        CourseCode = courseCode;
        Title = title;
        Description = description;
        Credits = credits;
        DepartmentId = departmentId;
        CoordinatorId = coordinatorId;
        MaxCapacity = maxCapacity;
        IsBorrowable = isBorrowable;
        OfferingSemester = offeringSemester;
        AcademicYear = academicYear;
        Prerequisites = prerequisites ?? new List<Guid>();
    }
}
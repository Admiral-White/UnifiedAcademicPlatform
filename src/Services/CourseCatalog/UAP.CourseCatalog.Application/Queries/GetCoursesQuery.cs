using MediatR;
using UAP.SharedKernel.Common;
using UAP.CourseCatalog.Application.DTOs;

namespace UAP.CourseCatalog.Application.Queries;

public class GetCoursesQuery : IRequest<Result<List<CourseDto>>>
{
    public string SearchTerm { get; set; }
    public Guid? DepartmentId { get; set; }
    public string Semester { get; set; }
    public int? AcademicYear { get; set; }
    public bool? IsBorrowable { get; set; }
    public bool? HasAvailableCapacity { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public GetCoursesQuery(
        string searchTerm = null,
        Guid? departmentId = null,
        string semester = null,
        int? academicYear = null,
        bool? isBorrowable = null,
        bool? hasAvailableCapacity = null,
        int page = 1,
        int pageSize = 20)
    {
        SearchTerm = searchTerm;
        DepartmentId = departmentId;
        Semester = semester;
        AcademicYear = academicYear;
        IsBorrowable = isBorrowable;
        HasAvailableCapacity = hasAvailableCapacity;
        Page = page;
        PageSize = pageSize;
    }
}
using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Infrastructure.Services;

public interface ICourseCatalogService
{
    Task<Result<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Result<List<CourseDto>>> GetCoursesByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCoursePrerequisitesAsync(Guid courseId, List<Guid> passedCourseIds, CancellationToken cancellationToken = default);
}

public class CourseDto
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid CoordinatorId { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public bool IsBorrowable { get; set; }
    public List<Guid> Prerequisites { get; set; } = new();
}
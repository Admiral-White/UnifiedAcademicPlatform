using UAP.CourseCatalog.Domain.Entities;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.CourseCatalog.Domain.Interfaces;
/// <summary>
/// Repository interface for Course aggregate root
/// Provides data access methods specific to Course entity
/// </summary>
public interface ICourseRepository : IRepository<Course>
{
    /// <summary>
    /// Gets a course by its unique course code
    /// </summary>
    /// <param name="courseCode">The course code to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The course if found, otherwise null</returns>
    Task<Course> GetByCodeAsync(string courseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a course by ID including all related details (prerequisites, department, etc.)
    /// </summary>
    /// <param name="id">The course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The course with all details if found, otherwise null</returns>
    Task<Course> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all courses belonging to a specific department
    /// </summary>
    /// <param name="departmentId">The department ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of courses in the department</returns>
    Task<IReadOnlyList<Course>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active courses
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active courses</returns>
    Task<IReadOnlyList<Course>> GetActiveCoursesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches courses by title, code, or description
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of courses matching the search criteria</returns>
    Task<IReadOnlyList<Course>> SearchCoursesAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets courses by semester and academic year
    /// </summary>
    /// <param name="semester">The semester</param>
    /// <param name="academicYear">The academic year</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of courses offered in the specified semester and year</returns>
    Task<IReadOnlyList<Course>> GetBySemesterAndYearAsync(string semester, int academicYear, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets courses that are available for borrowing (cross-department registration)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of borrowable courses</returns>
    Task<IReadOnlyList<Course>> GetBorrowableCoursesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a course exists with the specified course code
    /// </summary>
    /// <param name="courseCode">The course code to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a course exists with the code, otherwise false</returns>
    Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the current enrollment count for a course
    /// </summary>
    /// <param name="courseId">The course ID</param>
    /// <param name="newEnrollment">The new enrollment count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UpdateEnrollmentAsync(Guid courseId, int newEnrollment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets courses that have available capacity
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of courses with available seats</returns>
    Task<IReadOnlyList<Course>> GetCoursesWithAvailableCapacityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets courses by coordinator ID
    /// </summary>
    /// <param name="coordinatorId">The coordinator user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of courses coordinated by the specified user</returns>
    Task<IReadOnlyList<Course>> GetByCoordinatorAsync(Guid coordinatorId, CancellationToken cancellationToken = default);
}
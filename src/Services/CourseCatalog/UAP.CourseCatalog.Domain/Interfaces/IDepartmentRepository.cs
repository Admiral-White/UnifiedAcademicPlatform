using UAP.CourseCatalog.Domain.Entities;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.CourseCatalog.Domain.Interfaces;
/// <summary>
/// Repository interface for Department aggregate root
/// Provides data access methods specific to Department entity
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Gets a department by its unique code
    /// </summary>
    /// <param name="code">The department code to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The department if found, otherwise null</returns>
    Task<Department> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a department by ID including all its courses
    /// </summary>
    /// <param name="id">The department ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The department with courses if found, otherwise null</returns>
    Task<Department> GetByIdWithCoursesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active departments
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active departments</returns>
    Task<IReadOnlyList<Department>> GetActiveDepartmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a department exists with the specified code
    /// </summary>
    /// <param name="code">The department code to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a department exists with the code, otherwise false</returns>
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
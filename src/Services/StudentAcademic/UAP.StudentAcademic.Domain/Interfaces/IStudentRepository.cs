using UAP.Shared.Infrastructure.Interfaces;
using UAP.StudentAcademic.Domain.Entities;

namespace UAP.StudentAcademic.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
    Task<Student> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Student> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> SearchStudentsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ExistsByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetStudentsByCGPAAsync(decimal minCGPA, decimal maxCGPA, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetProbationStudentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetDeanListStudentsAsync(CancellationToken cancellationToken = default);
}
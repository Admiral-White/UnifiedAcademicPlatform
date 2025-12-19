using UAP.Shared.Infrastructure.Interfaces;
using UAP.StudentAcademic.Domain.Entities;

namespace UAP.StudentAcademic.Domain.Interfaces;

public interface ICourseGradeRepository : IRepository<CourseGrade>
{
    Task<CourseGrade> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseGrade>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseGrade>> GetByStudentAndSemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseGrade>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseGrade>> GetByCourseAndSemesterAsync(Guid courseId, string semester, int academicYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseGrade>> GetGradesByStatusAsync(Guid studentId, bool isFinal, CancellationToken cancellationToken = default);
    Task<decimal> GetStudentGPABySemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default);
    Task<bool> HasPassedCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
}
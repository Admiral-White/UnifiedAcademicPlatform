using UAP.Shared.Infrastructure.Interfaces;
using UAP.StudentAcademic.Domain.Entities;

namespace UAP.StudentAcademic.Domain.Interfaces;

public interface IAcademicSemesterRepository : IRepository<AcademicSemester>
{
    Task<AcademicSemester> GetCurrentSemesterAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademicSemester>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademicSemester>> GetByStudentAndYearAsync(Guid studentId, int academicYear, CancellationToken cancellationToken = default);
    Task<AcademicSemester> GetByStudentAndSemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademicSemester>> GetCompletedSemestersAsync(Guid studentId, CancellationToken cancellationToken = default);
}
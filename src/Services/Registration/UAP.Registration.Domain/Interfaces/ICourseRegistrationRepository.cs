using UAP.Registration.Domain.Entities;

namespace UAP.Registration.Domain.Interfaces;

public interface ICourseRegistrationRepository
{
    Task<CourseRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseRegistration>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseRegistration>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseRegistration?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, string semesterId, CancellationToken cancellationToken = default);
    Task AddAsync(CourseRegistration registration, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseRegistration registration, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, string semesterId, CancellationToken cancellationToken = default);
}

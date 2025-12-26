using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAP.StudentAcademic.Domain.Entities;
using UAP.StudentAcademic.Domain.Interfaces;
using UAP.StudentAcademic.Infrastructure.Data;

namespace UAP.StudentAcademic.Infrastructure.Repositories;

public class AcademicSemesterRepository : IAcademicSemesterRepository
{
    private readonly StudentAcademicDbContext _context;
    private readonly ILogger<AcademicSemesterRepository> _logger;

    public AcademicSemesterRepository(StudentAcademicDbContext context, ILogger<AcademicSemesterRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AcademicSemester> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AcademicSemester>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters.ToListAsync(cancellationToken);
    }

    public async Task<AcademicSemester> AddAsync(AcademicSemester entity, CancellationToken cancellationToken = default)
    {
        await _context.AcademicSemesters.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(AcademicSemester entity, CancellationToken cancellationToken = default)
    {
        _context.AcademicSemesters.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AcademicSemester entity, CancellationToken cancellationToken = default)
    {
        _context.AcademicSemesters.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<AcademicSemester> GetCurrentSemesterAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.IsCurrent, cancellationToken);
    }

    public async Task<IReadOnlyList<AcademicSemester>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.AcademicYear)
            .ThenBy(s => s.Semester)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademicSemester>> GetByStudentAndYearAsync(Guid studentId, int academicYear, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters
            .Where(s => s.StudentId == studentId && s.AcademicYear == academicYear)
            .OrderBy(s => s.Semester)
            .ToListAsync(cancellationToken);
    }

    public async Task<AcademicSemester> GetByStudentAndSemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.Semester == semester && s.AcademicYear == academicYear, cancellationToken);
    }

    public async Task<IReadOnlyList<AcademicSemester>> GetCompletedSemestersAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicSemesters
            .Where(s => s.StudentId == studentId && s.IsCompleted)
            .OrderByDescending(s => s.AcademicYear)
            .ThenBy(s => s.Semester)
            .ToListAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAP.StudentAcademic.Domain.Entities;
using UAP.StudentAcademic.Domain.Enums;
using UAP.StudentAcademic.Domain.Interfaces;
using UAP.StudentAcademic.Infrastructure.Data;

namespace UAP.StudentAcademic.Infrastructure.Repositories;

public class CourseGradeRepository : ICourseGradeRepository
{
    private readonly StudentAcademicDbContext _context;
    private readonly ILogger<CourseGradeRepository> _logger;

    public CourseGradeRepository(StudentAcademicDbContext context, ILogger<CourseGradeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseGrade> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting course grade by ID: {GradeId}", id);
        
        return await _context.CourseGrades
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<CourseGrade> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting course grade with details by ID: {GradeId}", id);
        
        return await _context.CourseGrades
            .Include(g => g.Student)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting grades for student: {StudentId}", studentId);
        
        return await _context.CourseGrades
            .Where(g => g.StudentId == studentId)
            .OrderByDescending(g => g.AcademicYear)
            .ThenBy(g => g.Semester)
            .ThenBy(g => g.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetByStudentAndSemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting grades for student {StudentId} in {Semester} {AcademicYear}", studentId, semester, academicYear);
        
        return await _context.CourseGrades
            .Where(g => g.StudentId == studentId && 
                       g.Semester == semester && 
                       g.AcademicYear == academicYear)
            .OrderBy(g => g.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting grades for course: {CourseId}", courseId);
        
        return await _context.CourseGrades
            .Where(g => g.CourseId == courseId)
            .OrderByDescending(g => g.AcademicYear)
            .ThenBy(g => g.Semester)
            .ThenBy(g => g.StudentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetByCourseAndSemesterAsync(Guid courseId, string semester, int academicYear, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting grades for course {CourseId} in {Semester} {AcademicYear}", courseId, semester, academicYear);
        
        return await _context.CourseGrades
            .Where(g => g.CourseId == courseId && 
                       g.Semester == semester && 
                       g.AcademicYear == academicYear)
            .OrderBy(g => g.StudentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all course grades");
        
        return await _context.CourseGrades
            .OrderByDescending(g => g.AcademicYear)
            .ThenBy(g => g.Semester)
            .ThenBy(g => g.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseGrade> AddAsync(CourseGrade courseGrade, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding grade for student {StudentId}, course {CourseCode}", 
            courseGrade.StudentId, courseGrade.CourseCode);
        
        await _context.CourseGrades.AddAsync(courseGrade, cancellationToken);
        return courseGrade;
    }

    public Task UpdateAsync(CourseGrade courseGrade, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating course grade: {GradeId}", courseGrade.Id);
        
        _context.CourseGrades.Update(courseGrade);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CourseGrade courseGrade, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting course grade: {GradeId}", courseGrade.Id);
        
        _context.CourseGrades.Remove(courseGrade);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CourseGrades.AnyAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CourseGrade>> GetGradesByStatusAsync(Guid studentId, bool isFinal, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting grades for student {StudentId} with final status: {IsFinal}", studentId, isFinal);
        
        return await _context.CourseGrades
            .Where(g => g.StudentId == studentId && g.IsFinal == isFinal)
            .OrderByDescending(g => g.AcademicYear)
            .ThenBy(g => g.Semester)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetStudentGPABySemesterAsync(Guid studentId, string semester, int academicYear, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Calculating GPA for student {StudentId} in {Semester} {AcademicYear}", studentId, semester, academicYear);
        
        var grades = await GetByStudentAndSemesterAsync(studentId, semester, academicYear, cancellationToken);
        
        var gradedCourses = grades
            .Where(g => g.Grade != Grade.W && g.Grade != Grade.I && g.Grade != Grade.P)
            .ToList();

        if (!gradedCourses.Any())
            return 0.0m;

        var totalGradePoints = gradedCourses.Sum(g => g.GetGradePoints());
        var totalCredits = gradedCourses.Sum(g => g.Credits);

        return totalCredits > 0 ? Math.Round(totalGradePoints / totalCredits, 2) : 0.0m;
    }

    public async Task<bool> HasPassedCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var grades = await _context.CourseGrades
            .Where(g => g.StudentId == studentId && g.CourseId == courseId)
            .ToListAsync(cancellationToken);

        return grades.Any(g => g.IsPassingGrade());
    }
}
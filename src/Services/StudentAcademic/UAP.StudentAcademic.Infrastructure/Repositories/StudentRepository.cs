using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAP.StudentAcademic.Domain.Entities;
using UAP.StudentAcademic.Domain.Enums;
using UAP.StudentAcademic.Domain.Interfaces;
using UAP.StudentAcademic.Infrastructure.Data;

namespace UAP.StudentAcademic.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly StudentAcademicDbContext _context;
    private readonly ILogger<StudentRepository> _logger;

    public StudentRepository(StudentAcademicDbContext context, ILogger<StudentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Student> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting student by ID: {StudentId}", id);
        
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Student> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting student by number: {StudentNumber}", studentNumber);
        
        return await _context.Students
            .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber, cancellationToken);
    }

    public async Task<Student> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting student by user ID: {UserId}", userId);
        
        return await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<Student> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting student with details by ID: {StudentId}", id);
        
        return await _context.Students
            .Include(s => s.Grades)
            .Include(s => s.AcademicSemesters)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting students by department: {DepartmentId}", departmentId);
        
        return await _context.Students
            .Where(s => s.DepartmentId == departmentId)
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting students by status: {Status}", status);
        
        if (!Enum.TryParse<StudentStatus>(status, true, out var statusEnum))
        {
            _logger.LogWarning("Invalid student status: {Status}", status);
            return new List<Student>();
        }

        return await _context.Students
            .Where(s => s.Status == statusEnum)
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> SearchStudentsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        _logger.LogDebug("Searching students with term: {SearchTerm}", searchTerm);
        
        return await _context.Students
            .Where(s => s.StudentNumber.Contains(searchTerm) ||
                       s.Program.Contains(searchTerm))
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all students");
        
        return await _context.Students
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding student: {StudentNumber}", student.StudentNumber);
        
        await _context.Students.AddAsync(student, cancellationToken);
        return student;
    }

    public Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating student: {StudentId}", student.Id);
        
        _context.Students.Update(student);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Student student, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting student: {StudentId}", student.Id);
        
        _context.Students.Remove(student);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(s => s.StudentNumber == studentNumber, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetStudentsByCGPAAsync(decimal minCGPA, decimal maxCGPA, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting students by CGPA range: {MinCGPA} - {MaxCGPA}", minCGPA, maxCGPA);
        
        return await _context.Students
            .Where(s => s.CurrentCGPA >= minCGPA && s.CurrentCGPA <= maxCGPA)
            .OrderByDescending(s => s.CurrentCGPA)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetProbationStudentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting probation students");
        
        return await _context.Students
            .Where(s => s.Status == StudentStatus.Probation || s.CurrentCGPA < 2.0m)
            .OrderBy(s => s.CurrentCGPA)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetDeanListStudentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting dean's list students");
        
        return await _context.Students
            .Where(s => s.CurrentCGPA >= 3.5m && s.Status == StudentStatus.Active)
            .OrderByDescending(s => s.CurrentCGPA)
            .ToListAsync(cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Enums;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.CourseCatalog.Infrastructure.Data;

namespace UAP.CourseCatalog.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseCatalogDbContext _context;
    private readonly ILogger<CourseRepository> _logger;

    public CourseRepository(CourseCatalogDbContext context, ILogger<CourseRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Course> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting course by ID: {CourseId}", id);
        
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course> GetByCodeAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = courseCode?.ToUpper() ?? throw new ArgumentNullException(nameof(courseCode));
        _logger.LogDebug("Getting course by code: {CourseCode}", normalizedCode);
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .FirstOrDefaultAsync(c => c.CourseCode == normalizedCode, cancellationToken);
    }

    public async Task<Course> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting course with details by ID: {CourseId}", id);
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .ThenInclude(p => p.PrerequisiteCourse)
            .Include(c => c.Department)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting courses by department: {DepartmentId}", departmentId);
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Where(c => c.DepartmentId == departmentId && c.IsActive)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetActiveCoursesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all active courses");
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> SearchCoursesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetActiveCoursesAsync(cancellationToken);
        }

        var normalizedSearchTerm = searchTerm.ToUpper();
        _logger.LogDebug("Searching courses with term: {SearchTerm}", normalizedSearchTerm);
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive && 
                       (c.CourseCode.Contains(normalizedSearchTerm) ||
                        c.Title.ToUpper().Contains(normalizedSearchTerm) ||
                        c.Description.ToUpper().Contains(normalizedSearchTerm)))
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetBySemesterAndYearAsync(string semester, int academicYear, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting courses for semester: {Semester}, year: {AcademicYear}", semester, academicYear);
        
        if (!Enum.TryParse<Semester>(semester, true, out var semesterEnum))
        {
            _logger.LogWarning("Invalid semester value: {Semester}", semester);
            return new List<Course>();
        }

        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive && 
                       c.OfferingSemester == semesterEnum && 
                       c.AcademicYear == academicYear)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetBorrowableCoursesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all borrowable courses");
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive && c.IsBorrowable)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetCoursesWithAvailableCapacityAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting courses with available capacity");
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive && c.CurrentEnrollment < c.MaxCapacity)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetByCoordinatorAsync(Guid coordinatorId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting courses by coordinator: {CoordinatorId}", coordinatorId);
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .Where(c => c.IsActive && c.CoordinatorId == coordinatorId)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all courses");
        
        return await _context.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.Department)
            .OrderBy(c => c.CourseCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new course: {CourseCode} - {Title}", course.CourseCode, course.Title);
        
        await _context.Courses.AddAsync(course, cancellationToken);
        return course;
    }

    public Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating course: {CourseId}", course.Id);
        
        _context.Courses.Update(course);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Course course, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting course: {CourseId} - {CourseCode}", course.Id, course.CourseCode);
        
        _context.Courses.Remove(course);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = courseCode?.ToUpper() ?? throw new ArgumentNullException(nameof(courseCode));
        return await _context.Courses.AnyAsync(c => c.CourseCode == normalizedCode, cancellationToken);
    }

    public async Task UpdateEnrollmentAsync(Guid courseId, int newEnrollment, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating enrollment for course: {CourseId} to {NewEnrollment}", courseId, newEnrollment);
        
        var course = await _context.Courses.FindAsync(new object[] { courseId }, cancellationToken);
        if (course != null)
        {
            var currentEnrollment = course.CurrentEnrollment;
            var difference = newEnrollment - currentEnrollment;
            
            // Use domain methods to maintain business rules
            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    var result = course.EnrollStudent();
                    if (result.IsFailure) break;
                }
            }
            else if (difference < 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    var result = course.DropStudent();
                    if (result.IsFailure) break;
                }
            }
            
            _context.Courses.Update(course);
            
            _logger.LogInformation("Updated enrollment for course {CourseId}: {OldEnrollment} -> {NewEnrollment}", 
                courseId, currentEnrollment, course.CurrentEnrollment);
        }
        else
        {
            _logger.LogWarning("Course not found for enrollment update: {CourseId}", courseId);
        }
    }
}
using Microsoft.Extensions.Logging;
using UAP.StudentAcademic.Domain.Interfaces;

namespace UAP.StudentAcademic.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly StudentAcademicDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    
    // Repository properties
    public IStudentRepository StudentRepository { get; }
    public ICourseGradeRepository CourseGradeRepository { get; }
    public IAcademicSemesterRepository AcademicSemesterRepository { get; }

    public UnitOfWork(
        StudentAcademicDbContext context, 
        ILogger<UnitOfWork> logger,
        IStudentRepository studentRepository,
        ICourseGradeRepository courseGradeRepository,
        IAcademicSemesterRepository academicSemesterRepository)
    {
        _context = context;
        _logger = logger;
        StudentRepository = studentRepository;
        CourseGradeRepository = courseGradeRepository;
        AcademicSemesterRepository = academicSemesterRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving changes to the database");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes to the database");
            throw;
        }
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving entities to the database");
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving entities to the database");
            return false;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
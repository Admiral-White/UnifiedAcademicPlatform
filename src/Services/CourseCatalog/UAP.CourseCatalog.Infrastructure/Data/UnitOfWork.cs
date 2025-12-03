using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.CourseCatalog.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly CourseCatalogDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    
    // Repository properties
    public ICourseRepository CourseRepository { get; }
    public IDepartmentRepository DepartmentRepository { get; }

    public UnitOfWork(
        CourseCatalogDbContext context, 
        ILogger<UnitOfWork> logger,
        ICourseRepository courseRepository,
        IDepartmentRepository departmentRepository)
    {
        _context = context;
        _logger = logger;
        CourseRepository = courseRepository;
        DepartmentRepository = departmentRepository;
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
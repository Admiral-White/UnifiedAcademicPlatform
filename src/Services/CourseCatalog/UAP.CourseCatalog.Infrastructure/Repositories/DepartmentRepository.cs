using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.CourseCatalog.Infrastructure.Data;

namespace UAP.CourseCatalog.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly CourseCatalogDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(CourseCatalogDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Department> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting department by ID: {DepartmentId}", id);
        
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Department> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        _logger.LogDebug("Getting department by code: {DepartmentCode}", normalizedCode);
        
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Code == normalizedCode, cancellationToken);
    }

    public async Task<Department> GetByIdWithCoursesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting department with courses by ID: {DepartmentId}", id);
        
        return await _context.Departments
            .Include(d => d.Courses)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all departments");
        
        return await _context.Departments
            .OrderBy(d => d.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Department>> GetActiveDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all active departments");
        
        return await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new department: {Code} - {Name}", department.Code, department.Name);
        
        await _context.Departments.AddAsync(department, cancellationToken);
        return department;
    }

    public Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating department: {DepartmentId}", department.Id);
        
        _context.Departments.Update(department);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Department department, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting department: {DepartmentId} - {Code}", department.Id, department.Code);
        
        _context.Departments.Remove(department);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        return await _context.Departments.AnyAsync(d => d.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Department>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await GetActiveDepartmentsAsync(cancellationToken);
    }
}
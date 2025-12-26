using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Queries;
using UAP.CourseCatalog.Application.DTOs;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.QueryHandlers;

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentDto>>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<GetDepartmentsQueryHandler> _logger;

    public GetDepartmentsQueryHandler(IDepartmentRepository departmentRepository, ILogger<GetDepartmentsQueryHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<List<DepartmentDto>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting departments - IncludeInactive: {IncludeInactive}", request.IncludeInactive);

            var departments = request.IncludeInactive 
                ? await _departmentRepository.GetAllAsync(cancellationToken)
                : await _departmentRepository.GetActiveAsync(cancellationToken);

            var departmentDtos = departments.Select(dept => new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code,
                Description = dept.Description,
                IsActive = dept.IsActive,
                CreatedAt = dept.CreatedOn,
                UpdatedAt = dept.ModifiedOn,
                CourseCount = dept.Courses?.Count ?? 0
            }).ToList();

            return Result.Success(departmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Result.Failure<List<DepartmentDto>>("An error occurred while retrieving departments");
        }
    }
}
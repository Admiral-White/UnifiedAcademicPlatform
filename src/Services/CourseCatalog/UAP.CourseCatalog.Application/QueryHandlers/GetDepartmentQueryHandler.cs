using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Queries;
using UAP.CourseCatalog.Application.DTOs;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.QueryHandlers;

public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, Result<DepartmentDto>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<GetDepartmentQueryHandler> _logger;

    public GetDepartmentQueryHandler(IDepartmentRepository departmentRepository, ILogger<GetDepartmentQueryHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<DepartmentDto>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting department: {DepartmentId}", request.DepartmentId);

            var department = await _departmentRepository.GetByIdWithCoursesAsync(request.DepartmentId, cancellationToken);
            if (department == null)
                return Result.Failure<DepartmentDto>("Department not found");

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                HeadOfDepartmentId = department.HeadOfDepartmentId,
                HeadOfDepartmentName = "Unknown", // Would come from user service
                IsActive = department.IsActive,
                CreatedAt = department.CreatedOn,
                UpdatedAt = department.ModifiedOn,
                CourseCount = department.Courses?.Count ?? 0,
                Courses = department.Courses?.Select(course => new CourseDto
                {
                    Id = course.Id,
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    Description = course.Description,
                    Credits = course.Credits,
                    DepartmentId = course.DepartmentId,
                    DepartmentName = department.Name,
                    CoordinatorId = course.CoordinatorId,
                    MaxCapacity = course.MaxCapacity,
                    CurrentEnrollment = course.CurrentEnrollment,
                    IsActive = course.IsActive,
                    IsBorrowable = course.IsBorrowable,
                    OfferingSemester = course.OfferingSemester,
                    AcademicYear = course.AcademicYear,
                    CreatedAt = course.CreatedOn,
                    UpdatedAt = course.ModifiedOn
                }).ToList() ?? new List<CourseDto>()
            };

            return Result.Success(departmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department: {DepartmentId}", request.DepartmentId);
            return Result.Failure<DepartmentDto>("An error occurred while retrieving the department");
        }
    }
}
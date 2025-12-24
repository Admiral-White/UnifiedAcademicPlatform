using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Queries;
using UAP.CourseCatalog.Application.DTOs;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.QueryHandlers;

public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, Result<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<GetCourseQueryHandler> _logger;

    public GetCourseQueryHandler(ICourseRepository courseRepository, ILogger<GetCourseQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<CourseDto>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting course: {CourseId}", request.CourseId);

            var course = await _courseRepository.GetByIdWithDetailsAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure<CourseDto>("Course not found");

            var courseDto = new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId,
                DepartmentName = course.Department?.Name ?? "Unknown",
                CoordinatorId = course.CoordinatorId,
                CoordinatorName = "Unknown Coordinator", // Would come from user service
                MaxCapacity = course.MaxCapacity,
                CurrentEnrollment = course.CurrentEnrollment,
                IsActive = course.IsActive,
                IsBorrowable = course.IsBorrowable,
                OfferingSemester = course.OfferingSemester,
                AcademicYear = course.AcademicYear,
                CreatedAt = course.CreatedOn,
                UpdatedAt = course.ModifiedOn,
                Prerequisites = course.Prerequisites.Select(p => new PrerequisiteDto
                {
                    CourseId = p.PrerequisiteCourseId,
                    CourseCode = p.PrerequisiteCourse?.CourseCode ?? "Unknown",
                    CourseTitle = p.PrerequisiteCourse?.Title ?? "Unknown"
                }).ToList()
            };

            return Result.Success(courseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course: {CourseId}", request.CourseId);
            return Result.Failure<CourseDto>("An error occurred while retrieving the course");
        }
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Queries;
using UAP.CourseCatalog.Application.DTOs;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.QueryHandlers;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Result<List<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<GetCoursesQueryHandler> _logger;

    public GetCoursesQueryHandler(ICourseRepository courseRepository, ILogger<GetCoursesQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<List<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting courses with filters - Search: {SearchTerm}, Department: {DepartmentId}", 
                request.SearchTerm, request.DepartmentId);

            IReadOnlyList<Course> courses;

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                courses = await _courseRepository.SearchCoursesAsync(request.SearchTerm, cancellationToken);
            }
            else if (request.DepartmentId.HasValue)
            {
                courses = await _courseRepository.GetByDepartmentAsync(request.DepartmentId.Value, cancellationToken);
            }
            else if (!string.IsNullOrEmpty(request.Semester) && request.AcademicYear.HasValue)
            {
                courses = await _courseRepository.GetBySemesterAndYearAsync(request.Semester, request.AcademicYear.Value, cancellationToken);
            }
            else if (request.IsBorrowable.HasValue && request.IsBorrowable.Value)
            {
                courses = await _courseRepository.GetBorrowableCoursesAsync(cancellationToken);
            }
            else if (request.HasAvailableCapacity.HasValue && request.HasAvailableCapacity.Value)
            {
                courses = await _courseRepository.GetCoursesWithAvailableCapacityAsync(cancellationToken);
            }
            else
            {
                courses = await _courseRepository.GetActiveCoursesAsync(cancellationToken);
            }

            // Apply pagination
            var paginatedCourses = courses
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var courseDtos = paginatedCourses.Select(course => new CourseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId,
                DepartmentName = course.Department?.Name ?? "Unknown",
                CoordinatorId = course.CoordinatorId,
                MaxCapacity = course.MaxCapacity,
                CurrentEnrollment = course.CurrentEnrollment,
                IsActive = course.IsActive,
                IsBorrowable = course.IsBorrowable,
                OfferingSemester = course.OfferingSemester,
                AcademicYear = course.AcademicYear,
                CreatedAt = course.CreatedOn,
                UpdatedAt = course.ModifiedOn
            }).ToList();

            return Result.Success(courseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses");
            return Result.Failure<List<CourseDto>>("An error occurred while retrieving courses");
        }
    }
}
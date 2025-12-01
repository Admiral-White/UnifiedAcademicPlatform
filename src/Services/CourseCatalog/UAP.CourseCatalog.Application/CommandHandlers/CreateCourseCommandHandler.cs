using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.CommandHandlers;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new course: {CourseCode}", request.CourseCode);

            // Check if the course code already exists
            var existingCourse = await _courseRepository.GetByCodeAsync(request.CourseCode, cancellationToken);
            if (existingCourse != null)
                return Result<Guid>.Failure($"Course with code {request.CourseCode} already exists") as Result<Guid>;

            // Verify department exists and is active
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department == null)
                return Result<Guid>.Failure("Department not found") as Result<Guid>;
            if (!department.IsActive)
                return Result<Guid>.Failure("Department is not active") as Result<Guid>;

            // Validate credits
            if (request.Credits <= 0)
                return Result<Guid>.Failure("Credits must be positive") as Result<Guid>;

            // Validate capacity
            if (request.MaxCapacity <= 0)
                return Result<Guid>.Failure("Max capacity must be positive") as Result<Guid>;

            // Create course
            var course = new Course(
                request.CourseCode,
                request.Title,
                request.Description,
                request.Credits,
                request.DepartmentId,
                request.CoordinatorId,
                request.MaxCapacity,
                request.IsBorrowable,
                request.OfferingSemester,
                request.AcademicYear
            );

            // Add prerequisites
            foreach (var prerequisiteId in request.Prerequisites)
            {
                var prerequisiteCourse = await _courseRepository.GetByIdAsync(prerequisiteId, cancellationToken);
                if (prerequisiteCourse == null)
                {
                    _logger.LogWarning("Prerequisite course not found: {PrerequisiteId}", prerequisiteId);
                    continue;
                }
                
                if (!prerequisiteCourse.IsActive)
                {
                    _logger.LogWarning("Prerequisite course is not active: {PrerequisiteId}", prerequisiteId);
                    continue;
                }

                course.AddPrerequisite(prerequisiteId);
            }

            // Save course
            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course created successfully: {CourseId} - {CourseCode}", course.Id, course.CourseCode);
            
            return Result<Guid>.Success(course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course: {CourseCode}", request.CourseCode);
            return Result<Guid>.Failure("An error occurred while creating the course") as Result<Guid>;
        }
    }
}
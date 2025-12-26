using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.CommandHandlers;

public class UpdateCourseCapacityCommandHandler : IRequestHandler<UpdateCourseCapacityCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseCapacityCommandHandler> _logger;

    public UpdateCourseCapacityCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCourseCapacityCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCourseCapacityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Course not found");

            if (!course.IsActive)
                return Result.Failure("Cannot update capacity for inactive course");

            if (request.NewCapacity <= 0)
                return Result.Failure("Capacity must be positive");

            if (request.NewCapacity < course.CurrentEnrollment)
                return Result.Failure("New capacity cannot be less than current enrollment");

            course.UpdateCapacity(request.NewCapacity);

            await _courseRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course capacity updated: {CourseId} - New capacity: {NewCapacity}", 
                request.CourseId, request.NewCapacity);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course capacity: {CourseId}", request.CourseId);
            return Result.Failure("An error occurred while updating course capacity");
        }
    }
}
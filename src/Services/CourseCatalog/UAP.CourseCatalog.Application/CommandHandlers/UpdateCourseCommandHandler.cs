using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.CommandHandlers;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating course: {CourseId}", request.CourseId);

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null || !course.IsActive)
                return Result.Failure("Course not found");

            // Update course details
            var updateResult = course.UpdateDetails(request.Title, request.Description, request.Credits);
            if (updateResult.IsFailure)
                return updateResult;

            // Update capacity if changed
            if (course.MaxCapacity != request.MaxCapacity)
            {
                var capacityResult = course.UpdateCapacity(request.MaxCapacity);
                if (capacityResult.IsFailure)
                    return capacityResult;
            }

            // Update borrowable status
            // Note: In a real scenario, we might have business rules for this

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course updated successfully: {CourseId}", request.CourseId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course: {CourseId}", request.CourseId);
            return Result.Failure("An error occurred while updating the course");
        }
    }
}
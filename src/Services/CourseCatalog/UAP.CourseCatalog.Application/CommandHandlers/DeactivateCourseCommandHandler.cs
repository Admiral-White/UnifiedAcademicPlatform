using MediatR;
using Microsoft.Extensions.Logging;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.CommandHandlers;

public class DeactivateCourseCommandHandler : IRequestHandler<DeactivateCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateCourseCommandHandler> _logger;

    public DeactivateCourseCommandHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeactivateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Course not found");

            if (!course.IsActive)
                return Result.Failure("Course is already deactivated");

            course.Deactivate();

            await _courseRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course deactivated: {CourseId} - Reason: {Reason}", 
                request.CourseId, request.Reason);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating course: {CourseId}", request.CourseId);
            return Result.Failure("An error occurred while deactivating the course");
        }
    }
}
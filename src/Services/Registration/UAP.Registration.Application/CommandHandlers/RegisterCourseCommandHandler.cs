using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Registration.Application.Commands;
using UAP.Registration.Domain.Entities;
using UAP.Registration.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.CommandHandlers;

public class RegisterCourseCommandHandler : IRequestHandler<RegisterCourseCommand, Result<Guid>>
{
    private readonly ICourseRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCourseCommandHandler> _logger;

    public RegisterCourseCommandHandler(
        ICourseRegistrationRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCourseCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(RegisterCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Registering student {StudentId} for course {CourseId}", request.StudentId, request.CourseId);

            var exists = await _repository.ExistsAsync(request.StudentId, request.CourseId, request.SemesterId, cancellationToken);
            if (exists)
                return Result.Failure<Guid>("Student is already registered for this course in this semester");

            var registration = CourseRegistration.Create(
                request.StudentId,
                request.CourseId,
                request.SemesterId,
                request.AcademicYear,
                request.Type);

            await _repository.AddAsync(registration, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course registration created successfully: {RegistrationId}", registration.Id);

            return Result.Success(registration.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering student {StudentId} for course {CourseId}", request.StudentId, request.CourseId);
            return Result.Failure<Guid>("An error occurred while registering for the course");
        }
    }
}

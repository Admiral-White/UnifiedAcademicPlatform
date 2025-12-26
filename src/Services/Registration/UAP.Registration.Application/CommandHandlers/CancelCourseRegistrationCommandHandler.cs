using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Registration.Application.Commands;
using UAP.Registration.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.CommandHandlers;

public class CancelCourseRegistrationCommandHandler : IRequestHandler<CancelCourseRegistrationCommand, Result<Guid>>
{
    private readonly ICourseRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelCourseRegistrationCommandHandler> _logger;

    public CancelCourseRegistrationCommandHandler(ICourseRegistrationRepository repository, IUnitOfWork unitOfWork, ILogger<CancelCourseRegistrationCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CancelCourseRegistrationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Cancelling course registration with {registration_Id}", request.RegistrationId);
            var registration = await _repository.GetByIdAsync(request.RegistrationId, cancellationToken);
            if (registration == null)
                return Result.Failure<Guid>("Registration not found");

            registration.Cancel();
            await _repository.UpdateAsync(registration, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Course registration cancelled successfully: {RegistrationId}", registration.Id);

            return Result.Success(registration.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error cancelling course registration with {registration_Id}", request.RegistrationId);
            return Result.Failure<Guid>("An error occurred while cancelling the course registration");
            
        }
        
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Registration.Application.Commands;
using UAP.Registration.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.CommandHandlers;

public class ApproveCourseRegistrationCommandHandler : IRequestHandler<ApproveCourseRegistrationCommand, Result<Guid>>
{
    private readonly ICourseRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveCourseRegistrationCommandHandler> _logger;

    public ApproveCourseRegistrationCommandHandler(ICourseRegistrationRepository repository, IUnitOfWork unitOfWork, ILogger<ApproveCourseRegistrationCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(ApproveCourseRegistrationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Approving student with {registration_Id} by {approve_By}", request.RegistrationId, request.ApprovedBy);
            var registration = await _repository.GetByIdAsync(request.RegistrationId, cancellationToken);
            if (registration == null)
                return Result.Failure<Guid>("Registration not found");

            registration.Approve(request.ApprovedBy);
            await _repository.UpdateAsync(registration, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Student approved successfully: {RegistrationId}", registration.Id);

            return Result.Success(registration.Id);  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving student with {registration_Id} by {approve_By}", request.RegistrationId, request.ApprovedBy);
            return Result.Failure<Guid>("An error occurred while approving for the course");
            // return Result.Failure<bool>(ex.Message);
        }
        
    }
}
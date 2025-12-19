using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Registration.Application.Commands;
using UAP.Registration.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.CommandHandlers;

public class RejectCourseRegistrationCommandHandler : IRequestHandler<RejectCourseRegistrationCommand, Result<Guid>>
{
    private readonly ICourseRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectCourseRegistrationCommandHandler> _logger;

    public RejectCourseRegistrationCommandHandler(ICourseRegistrationRepository repository, IUnitOfWork unitOfWork, ILogger<RejectCourseRegistrationCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(RejectCourseRegistrationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var registration = await _repository.GetByIdAsync(request.RegistrationId, cancellationToken);
            if (registration == null)
                return Result.Failure<Guid>("Registration not found");

            registration.Reject(request.Reason);
            await _repository.UpdateAsync(registration, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(registration.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occured while rejecting course registration:");
            return Result.Failure<Guid>("An error occurred while rejecting the course registration");
        }
        
    }
}
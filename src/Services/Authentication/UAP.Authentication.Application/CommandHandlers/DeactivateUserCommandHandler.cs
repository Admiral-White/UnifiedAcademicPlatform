using MediatR;
using Microsoft.Extensions.Logging;
using UAP.Authentication.Application.Commands;
using UAP.Authentication.Domain.Interfaces;
using UAP.Shared.Infrastructure.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Application.CommandHandlers;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;

    public DeactivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        if (!user.IsActive)
            return Result.Failure("User is already deactivated");

        user.Deactivate();
        
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} deactivated. Reason: {Reason}", request.UserId, request.Reason);
        
        return Result.Success();
    }
}
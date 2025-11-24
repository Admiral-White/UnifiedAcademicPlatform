using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Application.Commands;

public class DeactivateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Reason { get; set; }

    public DeactivateUserCommand(Guid userId, string reason = null)
    {
        UserId = userId;
        Reason = reason;
    }
}
using MediatR;
using UAP.Authentication.Domain.Enums;
using UAP.SharedKernel.Common;

namespace UAP.Authentication.Domain.Events;

public class UserCreatedDomainEvent : DomainEvent, INotification
{
    public Guid UserId { get; }
    public string Email { get; }
    public UserType UserType { get; }

    public UserCreatedDomainEvent(Guid userId, string email, UserType userType)
    {
        UserId = userId;
        Email = email;
        UserType = userType;
    }
}
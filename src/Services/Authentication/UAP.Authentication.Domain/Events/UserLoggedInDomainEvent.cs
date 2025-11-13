using UAP.SharedKernel.Common;

namespace UAP.Authentication.Domain.Events;

public class UserLoggedInDomainEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserLoggedInDomainEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
    
}
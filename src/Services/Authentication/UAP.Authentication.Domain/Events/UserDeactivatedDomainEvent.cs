using UAP.SharedKernel.Common;

namespace UAP.Authentication.Domain.Events;

public class UserDeactivatedDomainEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime DeactivatedAt { get; }

    public UserDeactivatedDomainEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        DeactivatedAt = DateTime.UtcNow;
    }
}
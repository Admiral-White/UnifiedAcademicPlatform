using System;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public class UserDeactivatedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        public string UserType { get; private set; }
        public DateTime DeactivatedAt { get; private set; }

        public UserDeactivatedIntegrationEvent(
            Guid userId,
            string email,
            string userType,
            DateTime deactivatedAt) : base()
        {
            UserId = userId;
            Email = email;
            UserType = userType;
            DeactivatedAt = deactivatedAt;
        }
    }
}
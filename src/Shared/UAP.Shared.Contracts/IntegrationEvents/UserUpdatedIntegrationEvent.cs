using System;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public class UserUpdatedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        public string UpdateType { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public UserUpdatedIntegrationEvent(
            Guid userId,
            string email,
            string updateType,
            DateTime updatedAt) : base()
        {
            UserId = userId;
            Email = email;
            UpdateType = updateType;
            UpdatedAt = updatedAt;
        }
    }
}
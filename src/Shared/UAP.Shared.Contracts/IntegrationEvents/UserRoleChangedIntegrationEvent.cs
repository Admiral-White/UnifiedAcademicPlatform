using System;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public class UserRoleChangedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string PreviousRole { get; private set; }
        public string NewRole { get; private set; }
        public DateTime ChangedAt { get; private set; }

        public UserRoleChangedIntegrationEvent(
            Guid userId,
            string previousRole,
            string newRole,
            DateTime changedAt) : base()
        {
            UserId = userId;
            PreviousRole = previousRole;
            NewRole = newRole;
            ChangedAt = changedAt;
        }
    }
}
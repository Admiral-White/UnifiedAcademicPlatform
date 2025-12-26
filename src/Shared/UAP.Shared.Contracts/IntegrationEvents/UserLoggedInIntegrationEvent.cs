using System;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public class UserLoggedInIntegrationEvent : BaseIntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        public DateTime LoggedInAt { get; private set; }

        public UserLoggedInIntegrationEvent(
            Guid userId,
            string email,
            DateTime loggedInAt) : base()
        {
            UserId = userId;
            Email = email;
            LoggedInAt = loggedInAt;
        }
    }
}
namespace UAP.Shared.Contracts.IntegrationEvents;

public class UserCreatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserType { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserCreatedIntegrationEvent(Guid userId, string email, string firstName, string lastName, string userType, DateTime createdAt)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        UserType = userType;
        CreatedAt = createdAt;
    }
}

public class UserLoggedInIntegrationEvent : BaseIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime LoginTime { get; set; }

    public UserLoggedInIntegrationEvent(Guid userId, string email, DateTime loginTime)
    {
        UserId = userId;
        Email = email;
        LoginTime = loginTime;
    }
}
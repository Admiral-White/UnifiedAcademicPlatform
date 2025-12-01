namespace UAP.Shared.Contracts.IntegrationEvents;

public class UserUpdatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserType { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdateType { get; set; } // "Profile", "Role", "Status", etc.

    public UserUpdatedIntegrationEvent(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string userType,
        DateTime updatedAt,
        string updateType)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        UserType = userType;
        UpdatedAt = updatedAt;
        UpdateType = updateType;
    }
}

public class UserRoleChangedIntegrationEvent : BaseIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string PreviousRole { get; set; }
    public string NewRole { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid ChangedBy { get; set; }

    public UserRoleChangedIntegrationEvent(
        Guid userId,
        string email,
        string previousRole,
        string newRole,
        DateTime changedAt,
        Guid changedBy)
    {
        UserId = userId;
        Email = email;
        PreviousRole = previousRole;
        NewRole = newRole;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
    }
}

public class UserPasswordChangedIntegrationEvent : BaseIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime ChangedAt { get; set; }
    public bool IsSelfService { get; set; }

    public UserPasswordChangedIntegrationEvent(
        Guid userId,
        string email,
        DateTime changedAt,
        bool isSelfService)
    {
        UserId = userId;
        Email = email;
        ChangedAt = changedAt;
        IsSelfService = isSelfService;
    }
}
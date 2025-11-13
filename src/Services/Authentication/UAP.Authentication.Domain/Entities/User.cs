using UAP.Authentication.Domain.Enums;
using UAP.Authentication.Domain.Events;
using UAP.SharedKernel.Entity;

namespace UAP.Authentication.Domain.Entities;

public class User  : AggregateRoot<Guid>
{
    public string Email { get; private set; }
    public string NormalizedEmail { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }
    public UserType UserType { get; private set; }
    
    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    // Private constructor for EF Core
    private User() { }

    public User(string email, string firstName, string lastName, UserType userType)
    {
        Id = Guid.NewGuid();
        Email = email?.ToLower() ?? throw new ArgumentNullException(nameof(email));
        NormalizedEmail = email.ToUpperInvariant();
        FirstName = firstName;
        LastName = lastName;
        UserType = userType;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserCreatedDomainEvent(Id, Email, UserType));
    }

    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
        PasswordHash = passwordHash;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AddDomainEvent(new UserLoggedInDomainEvent(Id, Email));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserDeactivatedDomainEvent(Id, Email));
    }

    public void AddRole(Guid roleId)
    {
        if (_roles.Any(r => r.RoleId == roleId))
            return;
            
        _roles.Add(new UserRole(Id, roleId));
    }

    public void RemoveRole(Guid roleId)
    {
        var role = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (role != null)
            _roles.Remove(role);
    }
}

public class UserRole : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; }
    public Role Role { get; private set; }

    private UserRole() { }

    public UserRole(Guid userId, Guid roleId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }
}
using UAP.SharedKernel.Entity;

namespace UAP.Authentication.Domain.Entities;

public class Role : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string NormalizedName { get; private set; }
    public bool IsSystemRole { get; private set; }
    
    private readonly List<RolePermission> _permissions = new();
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public Role(string name, string description, bool isSystemRole = false)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        NormalizedName = name.ToUpperInvariant();
        IsSystemRole = isSystemRole;
    }

    public void AddPermission(string permission)
    {
        if (_permissions.Any(p => p.Permission == permission))
            return;
            
        _permissions.Add(new RolePermission(Id, permission));
    }

    public void RemovePermission(string permission)
    {
        var existing = _permissions.FirstOrDefault(p => p.Permission == permission);
        if (existing != null)
            _permissions.Remove(existing);
    }
}

public class RolePermission : Entity<Guid>
{
    public Guid RoleId { get; private set; }
    public string Permission { get; private set; }

    // Navigation property
    public Role Role { get; private set; }

    private RolePermission() { }

    public RolePermission(Guid roleId, string permission)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        Permission = permission;
    }
}
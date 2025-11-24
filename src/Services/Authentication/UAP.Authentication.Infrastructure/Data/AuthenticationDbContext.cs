using Microsoft.EntityFrameworkCore;
using UAP.Authentication.Domain.Entities;
using UAP.Shared.Infrastructure.Data;

namespace UAP.Authentication.Infrastructure.Data;

public class AuthenticationDbContext : BaseDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.NormalizedEmail).IsRequired().HasMaxLength(256);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.UserType).IsRequired();

            // Indexes
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.NormalizedEmail).IsUnique();
            entity.HasIndex(u => u.UserType);
            entity.HasIndex(u => u.IsActive);

            // Relationships
            entity.HasMany(u => u.Roles)
                  .WithOne(ur => ur.User)
                  .HasForeignKey(ur => ur.UserId);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.NormalizedName).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);

            entity.HasIndex(r => r.Name).IsUnique();
            entity.HasIndex(r => r.NormalizedName).IsUnique();

            entity.HasMany(r => r.Permissions)
                  .WithOne(rp => rp.Role)
                  .HasForeignKey(rp => rp.RoleId);
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);
            
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.Roles)
                  .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                  .WithMany()
                  .HasForeignKey(ur => ur.RoleId);
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => rp.Id);
            
            entity.HasIndex(rp => new { rp.RoleId, rp.Permission }).IsUnique();

            entity.Property(rp => rp.Permission).IsRequired().HasMaxLength(100);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed roles
        var roles = new[]
        {
            new Role(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Student", "Student role", true),
            new Role(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Staff", "Staff role", true),
            new Role(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Administrator", "Administrator role", true),
            new Role(Guid.Parse("44444444-4444-4444-4444-444444444444"), "CourseCoordinator", "Course Coordinator role", true)
        };

        modelBuilder.Entity<Role>().HasData(roles);

        // Seed permissions for each role
        var rolePermissions = new List<RolePermission>();
        var permissionId = 1;

        foreach (var role in roles)
        {
            var permissions = GetDefaultPermissions(role.Name);
            foreach (var permission in permissions)
            {
                rolePermissions.Add(new RolePermission(role.Id, permission) { Id = Guid.NewGuid() });
            }
        }

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }

    private IEnumerable<string> GetDefaultPermissions(string roleName)
    {
        return roleName switch
        {
            "Student" => new[] { "course.view", "course.register", "grade.view", "profile.view" },
            "Staff" => new[] { "course.view", "course.manage", "student.view", "profile.view" },
            "CourseCoordinator" => new[] { "course.view", "course.manage", "course.coordinate", "student.view", "grade.manage", "profile.view" },
            "Administrator" => new[] { "*" }, // Wildcard for all permissions
            _ => new[] { "profile.view" }
        };
    }
}
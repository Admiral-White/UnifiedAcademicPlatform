using Microsoft.EntityFrameworkCore;
using UAP.Authentication.Domain.Entities;
using UAP.Authentication.Domain.Interfaces;
using UAP.Authentication.Infrastructure.Data;

namespace UAP.Authentication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthenticationDbContext _context;

    public UserRepository(AuthenticationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _context.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Roles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateLastLoginAsync(Guid userId, DateTime loginTime, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            user.UpdateLastLogin();
            _context.Users.Update(user);
        }
    }
}
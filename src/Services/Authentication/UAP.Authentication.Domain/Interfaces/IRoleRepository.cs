using UAP.Authentication.Domain.Entities;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.Authentication.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
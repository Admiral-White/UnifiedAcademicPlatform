using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAP.SharedKernel.Entity;

namespace UAP.Shared.Infrastructure.Interfaces
{
    /// <summary>
    /// Generic repository interface following Repository pattern
    /// Provides common data operations for aggregates
    /// </summary>
    public interface IRepository<T> where T : AggregateRoot<Guid>
    {
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

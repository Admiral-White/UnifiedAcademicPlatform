using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAP.SharedKernel.Entity;

namespace UAP.Shared.Infrastructure.Data
{
    /// <summary>
    /// Base DbContext with common configuration for all services
    /// Implements common patterns and behaviors
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply entity configurations from the same assembly
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            // Configure soft delete global query filter
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events before saving
            await DispatchDomainEvents();

            // Apply audit tracking
            UpdateAuditFields();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type type)
        {
            // Create expression: e => !e.IsDeleted
            var parameter = System.Linq.Expressions.Expression.Parameter(type);
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var condition = System.Linq.Expressions.Expression.MakeBinary(
                System.Linq.Expressions.ExpressionType.Equal,
                property,
                System.Linq.Expressions.Expression.Constant(false));

            return System.Linq.Expressions.Expression.Lambda(condition, parameter);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<IAuditable>();
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = currentTime;
                    entry.Entity.CreatedBy = "system"; // Will be replaced with actual user from context
                }

                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    entry.Entity.ModifiedOn = currentTime;
                    entry.Entity.ModifiedBy = "system";
                }
            }
        }

        private async Task DispatchDomainEvents()
        {
            var entitiesWithEvents = ChangeTracker.Entries<Entity<Guid>>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                // In a real implementation, we would dispatch to a domain event dispatcher
                // For now, we'll clear the events
                entity.ClearDomainEvents();
            }
        }
    }
}

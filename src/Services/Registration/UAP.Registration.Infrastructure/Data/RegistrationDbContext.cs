using Microsoft.EntityFrameworkCore;
using UAP.Registration.Domain.Entities;
using UAP.Shared.Infrastructure.Data;

namespace UAP.Registration.Infrastructure.Data;

public class RegistrationDbContext : BaseDbContext
{
    public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options) : base(options) { }

    public DbSet<CourseRegistration> CourseRegistrations => Set<CourseRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.ToTable("CourseRegistrations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.SemesterId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AcademicYear).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.RegistrationDate).IsRequired();
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();

            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => new { e.StudentId, e.CourseId, e.SemesterId }).IsUnique();
        });
    }
}

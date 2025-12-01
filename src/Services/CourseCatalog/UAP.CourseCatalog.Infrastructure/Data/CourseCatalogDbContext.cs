using Microsoft.EntityFrameworkCore;
using UAP.CourseCatalog.Domain.Entities;
using UAP.CourseCatalog.Domain.Enums;
using UAP.Shared.Infrastructure.Data;

namespace UAP.CourseCatalog.Infrastructure.Data;

public class CourseCatalogDbContext : BaseDbContext
{
    public CourseCatalogDbContext(DbContextOptions<CourseCatalogDbContext> options) : base(options)
    {
    }

    // DbSets for entities
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Prerequisite> Prerequisites => Set<Prerequisite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCourseEntity(modelBuilder);
        ConfigureDepartmentEntity(modelBuilder);
        ConfigurePrerequisiteEntity(modelBuilder);
        
        SeedInitialData(modelBuilder);
    }

    private void ConfigureCourseEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            // Primary Key
            entity.HasKey(c => c.Id);
            
            // Properties configuration
            entity.Property(c => c.CourseCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToUpper(),
                    v => v);

            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            entity.Property(c => c.Credits)
                .IsRequired();

            entity.Property(c => c.DepartmentId)
                .IsRequired();

            entity.Property(c => c.CoordinatorId)
                .IsRequired();

            entity.Property(c => c.MaxCapacity)
                .IsRequired();

            entity.Property(c => c.CurrentEnrollment)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(c => c.IsBorrowable)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(c => c.OfferingSemester)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(c => c.AcademicYear)
                .IsRequired();

            // Indexes
            entity.HasIndex(c => c.CourseCode)
                .IsUnique()
                .HasDatabaseName("IX_Courses_CourseCode");

            entity.HasIndex(c => c.DepartmentId)
                .HasDatabaseName("IX_Courses_DepartmentId");

            entity.HasIndex(c => c.CoordinatorId)
                .HasDatabaseName("IX_Courses_CoordinatorId");

            entity.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Courses_IsActive");

            entity.HasIndex(c => new { c.OfferingSemester, c.AcademicYear })
                .HasDatabaseName("IX_Courses_Semester_Year");

            entity.HasIndex(c => new { c.IsActive, c.IsBorrowable })
                .HasDatabaseName("IX_Courses_Active_Borrowable");

            // Query filter for active courses only
            entity.HasQueryFilter(c => c.IsActive);

            // Relationships
            entity.HasMany(c => c.Prerequisites)
                .WithOne(p => p.Course)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Department>()
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureDepartmentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            // Primary Key
            entity.HasKey(d => d.Id);
            
            // Properties configuration
            entity.Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(10)
                .HasConversion(
                    v => v.ToUpper(),
                    v => v);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Description)
                .HasMaxLength(500);

            entity.Property(d => d.HeadOfDepartmentId)
                .IsRequired();

            entity.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            entity.HasIndex(d => d.Code)
                .IsUnique()
                .HasDatabaseName("IX_Departments_Code");

            entity.HasIndex(d => d.IsActive)
                .HasDatabaseName("IX_Departments_IsActive");

            // Query filter for active departments only
            entity.HasQueryFilter(d => d.IsActive);

            // Relationships
            entity.HasMany(d => d.Courses)
                .WithOne()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePrerequisiteEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prerequisite>(entity =>
        {
            // Primary Key
            entity.HasKey(p => p.Id);
            
            // Properties configuration
            entity.Property(p => p.CourseId)
                .IsRequired();

            entity.Property(p => p.PrerequisiteCourseId)
                .IsRequired();

            // Indexes - Ensure unique prerequisite relationships
            entity.HasIndex(p => new { p.CourseId, p.PrerequisiteCourseId })
                .IsUnique()
                .HasDatabaseName("IX_Prerequisites_Course_Prerequisite");

            // Self-referencing relationship for prerequisites
            entity.HasOne(p => p.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.PrerequisiteCourse)
                .WithMany()
                .HasForeignKey(p => p.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Seed departments
        var computerScienceDept = new Department(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "CS",
            "Computer Science Department",
            Guid.Parse("11111111-1111-1111-1111-111111111111"))
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        var mathematicsDept = new Department(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "MATH",
            "Mathematics Department",
            Guid.Parse("22222222-2222-2222-2222-222222222222"))
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };

        var physicsDept = new Department(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "PHYS",
            "Physics Department",
            Guid.Parse("33333333-3333-3333-3333-333333333333"))
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333")
        };

        modelBuilder.Entity<Department>().HasData(
            computerScienceDept,
            mathematicsDept,
            physicsDept
        );

        // Seed courses
        var programmingCourse = new Course(
            "CS101",
            "Introduction to Programming",
            "Fundamental concepts of programming and problem solving",
            3,
            computerScienceDept.Id,
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            50,
            true,
            Semester.Fall,
            DateTime.Now.Year)
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        };

        var dataStructuresCourse = new Course(
            "CS201",
            "Data Structures and Algorithms",
            "Study of fundamental data structures and algorithm analysis",
            4,
            computerScienceDept.Id,
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            40,
            true,
            Semester.Spring,
            DateTime.Now.Year)
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
        };

        var calculusCourse = new Course(
            "MATH101",
            "Calculus I",
            "Differential and integral calculus of single variable functions",
            4,
            mathematicsDept.Id,
            Guid.Parse("55555555-5555-5555-5555-555555555555"),
            60,
            true,
            Semester.Fall,
            DateTime.Now.Year)
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")
        };

        var physicsCourse = new Course(
            "PHYS101",
            "General Physics I",
            "Mechanics, heat, and sound",
            4,
            physicsDept.Id,
            Guid.Parse("66666666-6666-6666-6666-666666666666"),
            55,
            false,
            Semester.Fall,
            DateTime.Now.Year)
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")
        };

        modelBuilder.Entity<Course>().HasData(
            programmingCourse,
            dataStructuresCourse,
            calculusCourse,
            physicsCourse
        );

        // Seed prerequisites (Data Structures requires Programming)
        var dataStructuresPrerequisite = new Prerequisite(
            dataStructuresCourse.Id,
            programmingCourse.Id)
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")
        };

        modelBuilder.Entity<Prerequisite>().HasData(dataStructuresPrerequisite);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Add custom save changes logic if needed
        // For example, update timestamps, validate business rules, etc.
        
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Course && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                // Set initial values for new courses
                if (entityEntry.Entity is Course course)
                {
                    // Ensure course code is uppercase
                    if (!string.IsNullOrEmpty(course.CourseCode))
                    {
                        course.CourseCode = course.CourseCode.ToUpper();
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    
}
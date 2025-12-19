using Microsoft.EntityFrameworkCore;
using UAP.Shared.Infrastructure.Data;
using UAP.StudentAcademic.Domain.Entities;
using UAP.StudentAcademic.Domain.Enums;

namespace UAP.StudentAcademic.Infrastructure.Data;

public class StudentAcademicDbContext : BaseDbContext
{
    public StudentAcademicDbContext(DbContextOptions<StudentAcademicDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Student> Students => Set<Student>();
    public DbSet<CourseGrade> CourseGrades => Set<CourseGrade>();
    public DbSet<AcademicSemester> AcademicSemesters => Set<AcademicSemester>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureStudentEntity(modelBuilder);
        ConfigureCourseGradeEntity(modelBuilder);
        ConfigureAcademicSemesterEntity(modelBuilder);
        
        SeedInitialData(modelBuilder);
    }

    private void ConfigureStudentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            
            // Properties
            entity.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(s => s.UserId)
                .IsRequired();
                
            entity.Property(s => s.DepartmentId)
                .IsRequired();
                
            entity.Property(s => s.Program)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(s => s.EnrollmentDate)
                .IsRequired();
                
            entity.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
                
            entity.Property(s => s.CurrentCGPA)
                .IsRequired()
                .HasPrecision(3, 2);
                
            entity.Property(s => s.CumulativeCredits)
                .IsRequired()
                .HasPrecision(6, 2);
                
            entity.Property(s => s.CompletedCredits)
                .IsRequired()
                .HasPrecision(6, 2);
                
            // Indexes
            entity.HasIndex(s => s.StudentNumber)
                .IsUnique()
                .HasDatabaseName("IX_Students_StudentNumber");
                
            entity.HasIndex(s => s.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Students_UserId");
                
            entity.HasIndex(s => s.DepartmentId)
                .HasDatabaseName("IX_Students_DepartmentId");
                
            entity.HasIndex(s => s.Status)
                .HasDatabaseName("IX_Students_Status");
                
            entity.HasIndex(s => s.CurrentCGPA)
                .HasDatabaseName("IX_Students_CurrentCGPA");
                
            // Relationships
            entity.HasMany(s => s.Grades)
                .WithOne(g => g.Student)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(s => s.AcademicSemesters)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCourseGradeEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseGrade>(entity =>
        {
            entity.HasKey(g => g.Id);
            
            // Properties
            entity.Property(g => g.StudentId)
                .IsRequired();
                
            entity.Property(g => g.CourseId)
                .IsRequired();
                
            entity.Property(g => g.CourseCode)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(g => g.CourseTitle)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(g => g.Credits)
                .IsRequired();
                
            entity.Property(g => g.Grade)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);
                
            entity.Property(g => g.Semester)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(g => g.AcademicYear)
                .IsRequired();
                
            entity.Property(g => g.SubmittedById)
                .IsRequired();
                
            entity.Property(g => g.Remarks)
                .HasMaxLength(500);
                
            entity.Property(g => g.GradeDate)
                .IsRequired();
                
            entity.Property(g => g.IsFinal)
                .IsRequired();
                
            // Indexes
            entity.HasIndex(g => g.StudentId)
                .HasDatabaseName("IX_CourseGrades_StudentId");
                
            entity.HasIndex(g => g.CourseId)
                .HasDatabaseName("IX_CourseGrades_CourseId");
                
            entity.HasIndex(g => new { g.StudentId, g.CourseId, g.Semester, g.AcademicYear })
                .IsUnique()
                .HasDatabaseName("IX_CourseGrades_Student_Course_Semester_Year");
                
            entity.HasIndex(g => new { g.Semester, g.AcademicYear })
                .HasDatabaseName("IX_CourseGrades_Semester_Year");
        });
    }

    private void ConfigureAcademicSemesterEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicSemester>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            // Properties
            entity.Property(a => a.StudentId)
                .IsRequired();
                
            entity.Property(a => a.Semester)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(a => a.AcademicYear)
                .IsRequired();
                
            entity.Property(a => a.SemesterGPA)
                .HasPrecision(3, 2);
                
            // Indexes
            entity.HasIndex(a => a.StudentId)
                .HasDatabaseName("IX_AcademicSemesters_StudentId");
                
            entity.HasIndex(a => new { a.StudentId, a.Semester, a.AcademicYear })
                .IsUnique()
                .HasDatabaseName("IX_AcademicSemesters_Student_Semester_Year");
                
            entity.HasIndex(a => a.IsCurrent)
                .HasDatabaseName("IX_AcademicSemesters_IsCurrent");
                
            entity.HasIndex(a => a.IsCompleted)
                .HasDatabaseName("IX_AcademicSemesters_IsCompleted");
        });
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Seed sample student
        var sampleStudent = new Student(
            "240120-1234",
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("11111111-1111-1111-1111-111111111111"), // CS Department
            "Computer Science",
            new DateTime(2024, 1, 20))
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            CurrentCGPA = 3.75m,
            CumulativeCredits = 45,
            CompletedCredits = 45,
            Status = StudentStatus.Active
        };

        modelBuilder.Entity<Student>().HasData(sampleStudent);

        // Seed sample academic semester
        var fall2023 = new AcademicSemester(
            sampleStudent.Id,
            "Fall",
            2023,
            false)
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            IsCompleted = true,
            SemesterGPA = 3.8m,
            RegisteredCredits = 15,
            CompletedCredits = 15
        };

        var spring2024 = new AcademicSemester(
            sampleStudent.Id,
            "Spring",
            2024,
            true)
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            RegisteredCredits = 15
        };

        modelBuilder.Entity<AcademicSemester>().HasData(fall2023, spring2024);

        // Seed sample grades
        var grades = new[]
        {
            new CourseGrade(
                sampleStudent.Id,
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // CS101
                "CS101",
                "Introduction to Programming",
                3,
                Grade.A,
                "Fall",
                2023,
                Guid.Parse("22222222-2222-2222-2222-222222222222"), // Coordinator
                "Excellent performance"
            )
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")
            },
            new CourseGrade(
                sampleStudent.Id,
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // MATH101
                "MATH101",
                "Calculus I",
                4,
                Grade.BPlus,
                "Fall",
                2023,
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Good work"
            )
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")
            }
        };

        modelBuilder.Entity<CourseGrade>().HasData(grades);
    }
}
using UAP.SharedKernel.Entity;

namespace UAP.StudentAcademic.Domain.Entities;

public class AcademicSemester : AggregateRoot<Guid>, IAuditable
{
    public Guid StudentId { get; private set; }
    public string Semester { get; private set; } // Fall, Spring, Summer, Winter
    public int AcademicYear { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public bool IsCompleted { get; set; }
    public decimal SemesterGPA { get; set; }
    public int RegisteredCredits { get; set; }
    public int CompletedCredits { get; set; }
    
    // Audit properties
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }

    // Navigation property
    public virtual Student Student { get; private set; }

    // Private constructor for EF Core
    private AcademicSemester() { }
    
    public AcademicSemester(Guid studentId, string semester, int academicYear, bool isCurrent = false)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        Semester = semester ?? throw new ArgumentNullException(nameof(semester));
        AcademicYear = academicYear;
        IsCurrent = isCurrent;
        IsCompleted = false;
        SemesterGPA = 0.0m;
        RegisteredCredits = 0;
        CompletedCredits = 0;
        
        // Set dates based on semester
        SetSemesterDates();
        
        // Set audit fields
        CreatedOn = DateTime.UtcNow;
        CreatedBy = "system";
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = "system";
    }
    
    private void SetSemesterDates()
    {
        switch (Semester.ToLower())
        {
            case "fall":
                StartDate = new DateTime(AcademicYear, 8, 1);
                EndDate = new DateTime(AcademicYear, 12, 31);
                break;
            case "spring":
                StartDate = new DateTime(AcademicYear, 1, 1);
                EndDate = new DateTime(AcademicYear, 5, 31);
                break;
            case "summer":
                StartDate = new DateTime(AcademicYear, 5, 1);
                EndDate = new DateTime(AcademicYear, 8, 31);
                break;
            case "winter":
                StartDate = new DateTime(AcademicYear - 1, 12, 1);
                EndDate = new DateTime(AcademicYear, 1, 31);
                break;
        }
    }
    
    public void MarkAsCurrent()
    {
        IsCurrent = true;
        ModifiedOn = DateTime.UtcNow;
    }

    public void MarkAsCompleted(decimal semesterGPA, int completedCredits)
    {
        IsCurrent = false;
        IsCompleted = true;
        SemesterGPA = semesterGPA;
        CompletedCredits = completedCredits;
        ModifiedOn = DateTime.UtcNow;
    }
    
    public void UpdateRegisteredCredits(int registeredCredits)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot update credits for completed semester");

        RegisteredCredits = registeredCredits;
        ModifiedOn = DateTime.UtcNow;
    }

    public bool IsWithinSemester(DateTime date)
    {
        if (!StartDate.HasValue || !EndDate.HasValue)
            return false;

        return date >= StartDate.Value && date <= EndDate.Value;
    }
    
    public string GetDisplayName()
    {
        return $"{Semester} {AcademicYear}";
    }
}
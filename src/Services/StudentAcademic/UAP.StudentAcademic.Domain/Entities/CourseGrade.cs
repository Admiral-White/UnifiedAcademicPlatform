using UAP.SharedKernel.Common;
using UAP.SharedKernel.Entity;
using UAP.StudentAcademic.Domain.Enums;

namespace UAP.StudentAcademic.Domain.Entities;

public class CourseGrade : AggregateRoot<Guid>, IAuditable
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string CourseCode { get; private set; }
    public string CourseTitle { get; private set; }
    public int Credits { get; private set; }
    public Grade Grade { get; private set; }
    public string Semester { get; private set; }
    public int AcademicYear { get; private set; }
    public Guid SubmittedById { get; private set; }
    public string Remarks { get; private set; }
    public DateTime GradeDate { get; private set; }
    public bool IsFinal { get; private set; }
    
    // Audit properties
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    
    // Navigation property
    public virtual Student Student { get; private set; }

    // Private constructor for EF Core
    private CourseGrade() { }

    public CourseGrade(Guid studentId, Guid courseId, string courseCode, string courseTitle, int credits, Grade grade, string semester,
        int academicYear,
        Guid submittedById,
        string remarks = null)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        CourseId = courseId;
        CourseCode = courseCode ?? throw new ArgumentNullException(nameof(courseCode));
        CourseTitle = courseTitle ?? throw new ArgumentNullException(nameof(courseTitle));
        Credits = credits > 0 ? credits : throw new ArgumentException("Credits must be positive", nameof(credits));
        Grade = grade;
        Semester = semester ?? throw new ArgumentNullException(nameof(semester));
        AcademicYear = academicYear;
        SubmittedById = submittedById;
        Remarks = remarks;
        GradeDate = DateTime.UtcNow;
        IsFinal = true;
        
        // Set audit fields
        CreatedOn = DateTime.UtcNow;
        CreatedBy = "system";
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = "system";
    }
    
    public Result UpdateGrade(Grade newGrade, string reason, Guid updatedById)
    {
        if (IsFinal && !string.IsNullOrEmpty(reason))
        {
            // Allow grade updates with proper reason
            Grade = newGrade;
            Remarks = $"Grade updated: {reason}. Updated by: {updatedById} on {DateTime.UtcNow}";
            ModifiedOn = DateTime.UtcNow;
            ModifiedBy = updatedById.ToString();
            
            return Result.Success();
        }
        
        return Result.Failure("Cannot update final grade without reason");
    }
    
    public decimal GetGradePoints()
    {
        return Grade switch
        {
            Grade.A => 4.0m * Credits,
            Grade.BPlus => 3.5m * Credits,
            Grade.B => 3.0m * Credits,
            Grade.CPlus => 2.5m * Credits,
            Grade.C => 2.0m * Credits,
            Grade.D => 1.0m * Credits,
            Grade.F => 0.0m * Credits,
            _ => 0.0m // W, I, P don't contribute to GPA
        };
    }
    
    public decimal GetGradeValue()
    {
        return Grade switch
        {
            Grade.A => 4.0m,
            Grade.BPlus => 3.5m,
            Grade.B => 3.0m,
            Grade.CPlus => 2.5m,
            Grade.C => 2.0m,
            Grade.D => 1.0m,
            Grade.F => 0.0m,
            _ => 0.0m
        };
    }
    
    public bool IsPassingGrade()
    {
        return Grade switch
        {
            Grade.A or Grade.BPlus or Grade.B or Grade.CPlus or Grade.C or Grade.D or Grade.P => true,
            _ => false
        };
    }

    public void MarkAsIncomplete(string reason)
    {
        if (IsFinal)
            throw new InvalidOperationException("Cannot mark final grade as incomplete");

        Grade = Grade.I;
        Remarks = $"Incomplete: {reason}";
        IsFinal = false;
        ModifiedOn = DateTime.UtcNow;
    }
    
    public void CompleteIncomplete(Grade finalGrade, string completionNotes)
    {
        if (Grade != Grade.I)
            throw new InvalidOperationException("Only incomplete grades can be completed");

        Grade = finalGrade;
        Remarks += $"\nCompleted: {completionNotes}";
        IsFinal = true;
        ModifiedOn = DateTime.UtcNow;
    }
}
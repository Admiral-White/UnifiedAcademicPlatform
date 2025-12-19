using UAP.SharedKernel.Common;
using UAP.SharedKernel.Entity;
using UAP.StudentAcademic.Domain.Enums;
using UAP.StudentAcademic.Domain.Events;

namespace UAP.StudentAcademic.Domain.Entities;

public class Student : AggregateRoot<Guid>, IAuditable
{
    public string StudentNumber { get; private set; }
    public Guid UserId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public string Program { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? ExpectedGraduationDate { get; private set; }
    public StudentStatus Status { get; set; }
    public decimal CurrentCGPA { get; set; }
    public decimal CumulativeCredits { get; set; }
    public decimal CompletedCredits { get; set; }

    // Audit properties
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }

    private readonly List<CourseGrade> _grades = new();
    public IReadOnlyCollection<CourseGrade> Grades => _grades.AsReadOnly();

    private readonly List<AcademicSemester> _academicSemesters = new();
    public IReadOnlyCollection<AcademicSemester> AcademicSemesters => _academicSemesters.AsReadOnly();

    // Private constructor for EF Core
    private Student()
    {
    }

    public Student(string studentNumber, Guid userId, Guid departmentId, string program, DateTime enrollmentDate)
    {
        Id = Guid.NewGuid();
        StudentNumber = studentNumber ?? throw new ArgumentNullException(nameof(studentNumber));
        UserId = userId;
        DepartmentId = departmentId;
        Program = program ?? throw new ArgumentNullException(nameof(program));
        EnrollmentDate = enrollmentDate;
        Status = StudentStatus.Active;
        CurrentCGPA = 0.0m;
        CumulativeCredits = 0.0m;
        CompletedCredits = 0.0m;

        // Set expected graduation date (typically 4 years after enrollment)
        ExpectedGraduationDate = enrollmentDate.AddYears(4);

        // Set audit fields
        CreatedOn = DateTime.UtcNow;
        CreatedBy = "system";
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = "system";

        AddDomainEvent(new StudentCreatedDomainEvent(Id, StudentNumber, UserId));
    }

    public Result AddGrade(Guid courseId, string courseCode, string courseTitle, int credits, Grade grade,
        string semester, int academicYear, Guid submittedById, string requestRemarks)
    {
        if (Status != StudentStatus.Active && Status != StudentStatus.Probation)
            return Result.Failure("Student is not in an active status to receive grades");

        if (credits <= 0)
            return Result.Failure("Credits must be positive");

        // Check if grade already exists for this course in the same semester
        var existingGrade = _grades.FirstOrDefault(g =>
            g.CourseId == courseId &&
            g.Semester == semester &&
            g.AcademicYear == academicYear);

        if (existingGrade != null)
            return Result.Failure("Grade already exists for this course in the specified semester");

        var courseGrade = new CourseGrade(
            Id,
            courseId,
            courseCode,
            courseTitle,
            credits,
            grade,
            semester,
            academicYear,
            submittedById
        );

        _grades.Add(courseGrade);

        // Update CGPA and credits
        UpdateAcademicMetrics();

        AddDomainEvent(new GradeSubmittedDomainEvent(Id, courseId, grade.ToString(), semester, academicYear));

        return Result.Success();


    }
    
    public Result UpdateGrade(Guid gradeId, Grade newGrade, string reason, Guid updatedById)
    {
        var grade = _grades.FirstOrDefault(g => g.Id == gradeId);
        if (grade == null)
            return Result.Failure("Grade not found");

        var oldGrade = grade.Grade;
        
        var updateResult = grade.UpdateGrade(newGrade, reason, updatedById);
        if (updateResult.IsFailure)
            return updateResult;

        // Update CGPA and credits
        UpdateAcademicMetrics();
        
        return Result.Success();
    }
    
    public void UpdateStatus(StudentStatus newStatus, string reason)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        
        AddDomainEvent(new StudentStatusChangedDomainEvent(Id, oldStatus.ToString(), newStatus.ToString(), reason));
    }
    
    
    public Result<decimal> CalculateCurrentCGPA()
    {
        var gradedCourses = _grades
            .Where(g => g.Grade != Grade.W && g.Grade != Grade.I && g.Grade != Grade.P)
            .ToList();

        if (!gradedCourses.Any())
            return Result.Success(0.0m);

        var totalGradePoints = gradedCourses.Sum(g => g.GetGradePoints());
        var totalCredits = gradedCourses.Sum(g => g.Credits);

        if (totalCredits == 0)
            return Result.Success(0.0m);

        var cgpa = totalGradePoints / totalCredits;
        return Result.Success(Math.Round(cgpa, 2));
    }
    
    private void UpdateAcademicMetrics()
    {
        var cgpaResult = CalculateCurrentCGPA();
        if (cgpaResult.IsSuccess)
        {
            var oldCGPA = CurrentCGPA;
            CurrentCGPA = cgpaResult.Value;
            
            if (oldCGPA != CurrentCGPA)
            {
                AddDomainEvent(new CGPAUpdatedDomainEvent(Id, oldCGPA, CurrentCGPA));
            }
        }

        // Update credits
        CumulativeCredits = _grades.Sum(g => g.Credits);
        CompletedCredits = _grades
            .Where(g => g.Grade != Grade.F && g.Grade != Grade.W && g.Grade != Grade.I)
            .Sum(g => g.Credits);
            
        // Update status based on CGPA
        UpdateStatusBasedOnCGPA();
    }
    
    private void UpdateStatusBasedOnCGPA()
    {
        if (CurrentCGPA < 1.0m && Status == StudentStatus.Active)
        {
            UpdateStatus(StudentStatus.Probation, "CGPA below 1.0");
        }
        else if (CurrentCGPA >= 2.0m && Status == StudentStatus.Probation)
        {
            UpdateStatus(StudentStatus.Active, "CGPA improved to 2.0 or above");
        }
    }
    
    public Result<AcademicSemester> AddAcademicSemester(string semester, int academicYear, bool isCurrent = false)
    {
        var existingSemester = _academicSemesters.FirstOrDefault(s => 
            s.Semester == semester && s.AcademicYear == academicYear);
            
        if (existingSemester != null)
            return Result.Failure<AcademicSemester>("Academic semester already exists");

        var academicSemester = new AcademicSemester(Id, semester, academicYear, isCurrent);
        _academicSemesters.Add(academicSemester);
        
        return Result.Success(academicSemester);
    }
    
    public IEnumerable<CourseGrade> GetGradesBySemester(string semester, int academicYear)
    {
        return _grades.Where(g => g.Semester == semester && g.AcademicYear == academicYear);
    }
    
    
    public decimal GetSemesterGPA(string semester, int academicYear)
    {
        var semesterGrades = GetGradesBySemester(semester, academicYear)
            .Where(g => g.Grade != Grade.W && g.Grade != Grade.I && g.Grade != Grade.P)
            .ToList();

        if (!semesterGrades.Any())
            return 0.0m;

        var totalGradePoints = semesterGrades.Sum(g => g.GetGradePoints());
        var totalCredits = semesterGrades.Sum(g => g.Credits);

        return totalCredits > 0 ? Math.Round(totalGradePoints / totalCredits, 2) : 0.0m;
    }

    public bool HasPassedPrerequisite(Guid courseId)
    {
        // Check if student has passed (grade C or better) the prerequisite course
        return _grades.Any(g => 
            g.CourseId == courseId && 
            g.GetGradeValue() >= 2.0m); // C or better (2.0 on 4.0 scale)
    }
}




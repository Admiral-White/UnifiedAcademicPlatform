using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Application.Queries;

public class GetStudentQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; set; }

    public GetStudentQuery(Guid studentId)
    {
        StudentId = studentId;
    }
}

public class GetStudentByUserIdQuery : IRequest<Result<StudentDto>>
{
    public Guid UserId { get; set; }

    public GetStudentByUserIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetStudentGradesQuery : IRequest<Result<List<GradeDto>>>
{
    public Guid StudentId { get; set; }
    public string Semester { get; set; }
    public int? AcademicYear { get; set; }

    public GetStudentGradesQuery(Guid studentId, string semester = null, int? academicYear = null)
    {
        StudentId = studentId;
        Semester = semester;
        AcademicYear = academicYear;
    }
}

public class CalculateTranscriptQuery : IRequest<Result<TranscriptDto>>
{
    public Guid StudentId { get; set; }
    public bool IncludeInProgress { get; set; } = false;

    public CalculateTranscriptQuery(Guid studentId, bool includeInProgress = false)
    {
        StudentId = studentId;
        IncludeInProgress = includeInProgress;
    }
}


// DTOs
public class StudentDto
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; }
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string Program { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? ExpectedGraduationDate { get; set; }
    public string Status { get; set; }
    public decimal CurrentCGPA { get; set; }
    public decimal CumulativeCredits { get; set; }
    public decimal CompletedCredits { get; set; }
    public bool IsGoodStanding { get; set; }
    public bool IsDeanList { get; set; }
    public List<AcademicSemesterDto> AcademicSemesters { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

public class GradeDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public int Credits { get; set; }
    public string Grade { get; set; }
    public decimal GradePoints { get; set; }
    public string Semester { get; set; }
    public int AcademicYear { get; set; }
    public bool IsPassing { get; set; }
    public string Remarks { get; set; }
    public DateTime GradeDate { get; set; }
    public bool IsFinal { get; set; }
}

public class AcademicSemesterDto
{
    public Guid Id { get; set; }
    public string Semester { get; set; }
    public int AcademicYear { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsCompleted { get; set; }
    public decimal SemesterGPA { get; set; }
    public int RegisteredCredits { get; set; }
    public int CompletedCredits { get; set; }
    public string DisplayName { get; set; }
}

public class TranscriptDto
{
    public StudentDto Student { get; set; }
    public List<TranscriptSemesterDto> Semesters { get; set; } = new();
    public TranscriptSummaryDto Summary { get; set; }
}

public class TranscriptSemesterDto
{
    public string Semester { get; set; }
    public int AcademicYear { get; set; }
    public List<GradeDto> Grades { get; set; } = new();
    public decimal SemesterGPA { get; set; }
    public int TotalCredits { get; set; }
    public int EarnedCredits { get; set; }
}

public class TranscriptSummaryDto
{
    public decimal CumulativeGPA { get; set; }
    public decimal TotalCreditsAttempted { get; set; }
    public decimal TotalCreditsEarned { get; set; }
    public decimal CompletionPercentage { get; set; }
    public string AcademicStanding { get; set; }
    public List<string> Honors { get; set; } = new();
}


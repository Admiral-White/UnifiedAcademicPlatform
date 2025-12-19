using MediatR;
using Microsoft.Extensions.Logging;
using UAP.SharedKernel.Common;
using UAP.StudentAcademic.Application.Queries;
using UAP.StudentAcademic.Domain.Interfaces;

namespace UAP.StudentAcademic.Application.QueryHandlers;

public class CalculateTranscriptQueryHandler : IRequestHandler<CalculateTranscriptQuery, Result<TranscriptDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseGradeRepository _gradeRepository;
    private readonly IAcademicSemesterRepository _semesterRepository;
    private readonly ILogger<CalculateTranscriptQueryHandler> _logger;

    public CalculateTranscriptQueryHandler(
        IStudentRepository studentRepository,
        ICourseGradeRepository gradeRepository,
        IAcademicSemesterRepository semesterRepository,
        ILogger<CalculateTranscriptQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _gradeRepository = gradeRepository;
        _semesterRepository = semesterRepository;
        _logger = logger;
    }
    
    public async Task<Result<TranscriptDto>> Handle(CalculateTranscriptQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Calculating transcript for student: {StudentId}", request.StudentId);

            // Get student
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result<TranscriptDto>.Failure("Student not found") as Result<TranscriptDto>;

            // Get all semesters
            var semesters = await _semesterRepository.GetByStudentAsync(request.StudentId, cancellationToken);
            
            // Get all grades
            var grades = await _gradeRepository.GetByStudentAsync(request.StudentId, cancellationToken);

            // Group grades by semester
            var semesterGroups = grades
                .Where(g => request.IncludeInProgress || g.IsFinal)
                .GroupBy(g => new { g.Semester, g.AcademicYear })
                .OrderBy(g => g.Key.AcademicYear)
                .ThenBy(g => GetSemesterOrder(g.Key.Semester));

            var transcriptSemesters = new List<TranscriptSemesterDto>();
            
            foreach (var group in semesterGroups)
            {
                var semesterGrades = group.ToList();
                var semesterGPA = await _gradeRepository.GetStudentGPABySemesterAsync(
                    request.StudentId, group.Key.Semester, group.Key.AcademicYear, cancellationToken);
                
                transcriptSemesters.Add(new TranscriptSemesterDto
                {
                    Semester = group.Key.Semester,
                    AcademicYear = group.Key.AcademicYear,
                    Grades = semesterGrades.Select(g => new GradeDto
                    {
                        Id = g.Id,
                        CourseId = g.CourseId,
                        CourseCode = g.CourseCode,
                        CourseTitle = g.CourseTitle,
                        Credits = g.Credits,
                        Grade = g.Grade.ToString(),
                        GradePoints = g.GetGradePoints(),
                        Semester = g.Semester,
                        AcademicYear = g.AcademicYear,
                        IsPassing = g.IsPassingGrade(),
                        Remarks = g.Remarks,
                        GradeDate = g.GradeDate,
                        IsFinal = g.IsFinal
                    }).ToList(),
                    SemesterGPA = semesterGPA,
                    TotalCredits = semesterGrades.Sum(g => g.Credits),
                    EarnedCredits = semesterGrades.Where(g => g.IsPassingGrade()).Sum(g => g.Credits)
                });
            }

            // Calculate summary
            var allGrades = grades.Where(g => request.IncludeInProgress || g.IsFinal).ToList();
            var summary = CalculateTranscriptSummary(student, allGrades);

            var transcript = new TranscriptDto
            {
                Student = new StudentDto
                {
                    Id = student.Id,
                    StudentNumber = student.StudentNumber,
                    UserId = student.UserId,
                    DepartmentId = student.DepartmentId,
                    Program = student.Program,
                    EnrollmentDate = student.EnrollmentDate,
                    ExpectedGraduationDate = student.ExpectedGraduationDate,
                    Status = student.Status.ToString(),
                    CurrentCGPA = student.CurrentCGPA,
                    CumulativeCredits = student.CumulativeCredits,
                    CompletedCredits = student.CompletedCredits,
                    IsGoodStanding = student.CurrentCGPA >= 2.0m,
                    IsDeanList = student.CurrentCGPA >= 3.5m
                },
                Semesters = transcriptSemesters,
                Summary = summary
            };

            return Result<TranscriptDto>.Success(transcript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating transcript for student: {StudentId}", request.StudentId);
            return Result<TranscriptDto>.Failure("An error occurred while calculating the transcript") as Result<TranscriptDto>;
        }
    }
    
    private TranscriptSummaryDto CalculateTranscriptSummary(Domain.Entities.Student student, List<Domain.Entities.CourseGrade> grades)
    {
        var gradedCourses = grades.Where(g => g.IsFinal && g.Grade != Domain.Enums.Grade.W && 
                                              g.Grade != Domain.Enums.Grade.I && g.Grade != Domain.Enums.Grade.P).ToList();
        
        var totalCreditsAttempted = gradedCourses.Sum(g => g.Credits);
        var totalCreditsEarned = gradedCourses.Where(g => g.IsPassingGrade()).Sum(g => g.Credits);
        var totalGradePoints = gradedCourses.Sum(g => g.GetGradePoints());
        
        var cumulativeGPA = totalCreditsAttempted > 0 ? totalGradePoints / totalCreditsAttempted : 0.0m;
        var completionPercentage = totalCreditsAttempted > 0 ? (totalCreditsEarned / totalCreditsAttempted) * 100 : 0.0m;

        var honors = new List<string>();
        if (cumulativeGPA >= 3.5m) honors.Add("Dean's List");
        if (cumulativeGPA >= 3.8m) honors.Add("Summa Cum Laude");
        else if (cumulativeGPA >= 3.6m) honors.Add("Magna Cum Laude");
        else if (cumulativeGPA >= 3.4m) honors.Add("Cum Laude");

        return new TranscriptSummaryDto
        {
            CumulativeGPA = Math.Round(cumulativeGPA, 2),
            TotalCreditsAttempted = totalCreditsAttempted,
            TotalCreditsEarned = totalCreditsEarned,
            CompletionPercentage = Math.Round(completionPercentage, 2),
            AcademicStanding = cumulativeGPA >= 2.0m ? "Good Standing" : "Academic Probation",
            Honors = honors
        };
    }
    
    private int GetSemesterOrder(string semester)
    {
        return semester.ToLower() switch
        {
            "winter" => 1,
            "spring" => 2,
            "summer" => 3,
            "fall" => 4,
            _ => 5
        };
    }
}
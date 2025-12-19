using MediatR;
using Microsoft.Extensions.Logging;
using UAP.SharedKernel.Common;
using UAP.StudentAcademic.Application.Queries;
using UAP.StudentAcademic.Domain.Interfaces;

namespace UAP.StudentAcademic.Application.QueryHandlers;

public class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, Result<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<GetStudentQueryHandler> _logger;

    public GetStudentQueryHandler(
        IStudentRepository studentRepository,
        ILogger<GetStudentQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }
    
        public async Task<Result<StudentDto>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting student: {StudentId}", request.StudentId);

            var student = await _studentRepository.GetByIdWithDetailsAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result<StudentDto>.Failure("Student not found") as Result<StudentDto>;

            var studentDto = MapToStudentDto(student);
            
            return Result<StudentDto>.Success(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student: {StudentId}", request.StudentId);
            return Result<StudentDto>.Failure("An error occurred while retrieving the student") as Result<StudentDto>;
        }
    }

    private StudentDto MapToStudentDto(Domain.Entities.Student student)
    {
        return new StudentDto
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
            IsDeanList = student.CurrentCGPA >= 3.5m,
            AcademicSemesters = student.AcademicSemesters.Select(s => new AcademicSemesterDto
            {
                Id = s.Id,
                Semester = s.Semester,
                AcademicYear = s.AcademicYear,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsCurrent = s.IsCurrent,
                IsCompleted = s.IsCompleted,
                SemesterGPA = s.SemesterGPA,
                RegisteredCredits = s.RegisteredCredits,
                CompletedCredits = s.CompletedCredits,
                DisplayName = s.GetDisplayName()
            }).ToList(),
            CreatedOn = student.CreatedOn,
            ModifiedOn = student.ModifiedOn
        };
    }
}
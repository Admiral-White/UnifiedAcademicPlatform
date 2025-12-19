using MediatR;
using Microsoft.Extensions.Logging;
using UAP.SharedKernel.Common;
using UAP.StudentAcademic.Application.Commands;
using UAP.StudentAcademic.Domain.Enums;
using UAP.StudentAcademic.Domain.Interfaces;

namespace UAP.StudentAcademic.Application.CommandHandlers;

public class SubmitGradeCommandHandler : IRequestHandler<SubmitGradeCommand, Result<Guid>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitGradeCommandHandler> _logger;

    public SubmitGradeCommandHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitGradeCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(SubmitGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Submitting grade for student: {StudentId}, course: {CourseCode}, grade: {Grade}",
                request.StudentId, request.CourseCode, request.Grade);

            // Get student
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result<Guid>.Failure("Student not found") as Result<Guid>;

            // Parse grade
            if (!Enum.TryParse<Grade>(request.Grade, true, out var grade))
                return Result<Guid>.Failure($"Invalid grade: {request.Grade}") as Result<Guid>;

            // Submit grade
            var result = student.AddGrade(
                request.CourseId,
                request.CourseCode,
                request.CourseTitle,
                request.Credits,
                grade,
                request.Semester,
                request.AcademicYear,
                request.SubmittedById,
                request.Remarks
            );

            if (result.IsFailure)
                return Result<Guid>.Failure(result.Error) as Result<Guid>;

            // Update student
            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get the created grade ID (last added grade)
            var createdGrade = student.Grades.Last();
            
            _logger.LogInformation(
                "Grade submitted successfully: {GradeId} for student {StudentId}",
                createdGrade.Id, request.StudentId);
            
            return Result<Guid>.Success(createdGrade.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting grade for student: {StudentId}", request.StudentId);
            return Result<Guid>.Failure("An error occurred while submitting the grade") as Result<Guid>;
        }
    }
}
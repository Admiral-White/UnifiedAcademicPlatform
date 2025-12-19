using MediatR;
using Microsoft.Extensions.Logging;
using UAP.SharedKernel.Common;
using UAP.StudentAcademic.Application.Commands;
using UAP.StudentAcademic.Domain.Entities;
using UAP.StudentAcademic.Domain.Interfaces;

namespace UAP.StudentAcademic.Application.CommandHandlers;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<Guid>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating student with number: {StudentNumber}", request.StudentNumber);

            // Check if student number already exists
            var existingStudent =
                await _studentRepository.GetByStudentNumberAsync(request.StudentNumber, cancellationToken);
            if (existingStudent != null)
                return Result<Guid>.Failure($"Student with number {request.StudentNumber} already exists") as Result<Guid>;

            // Check if user already has a student record
            var existingUserStudent = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingUserStudent != null)
                return Result<Guid>.Failure($"User already has a student record") as Result<Guid>;

            // Create student
            var student = new Student(
                request.StudentNumber,
                request.UserId,
                request.DepartmentId,
                request.Program,
                request.EnrollmentDate
            );

            // Add current academic semester
            var currentDate = DateTime.UtcNow;
            var (semester, academicYear) = GetCurrentSemester(currentDate);

            var semesterResult = student.AddAcademicSemester(semester, academicYear, true);
            if (semesterResult.IsFailure)
            {
                _logger.LogWarning("Failed to add current semester: {Error}", semesterResult.Error);
            }

            // Save student
            await _studentRepository.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student created successfully: {StudentId} - {StudentNumber}", student.Id,
                student.StudentNumber);

            return Result<Guid>.Success(student.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student: {StudentNumber}", request.StudentNumber);
            return Result<Guid>.Failure("An error occurred while creating the student") as Result<Guid>;
        }
    }
    
    private (string Semester, int AcademicYear) GetCurrentSemester(DateTime date)
    {
        var month = date.Month;
        var year = date.Year;

        return month switch
        {
            >= 8 and <= 12 => ("Fall", year),
            >= 1 and <= 5 => ("Spring", year),
            >= 5 and <= 8 => ("Summer", year),
            _ => ("Winter", year)
        };
    }
}
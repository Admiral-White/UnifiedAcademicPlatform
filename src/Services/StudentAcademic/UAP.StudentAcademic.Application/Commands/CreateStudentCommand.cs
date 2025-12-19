using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Application.Commands;

public class CreateStudentCommand : IRequest<Result<Guid>>
{
    public string StudentNumber { get; set; }
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public string Program { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public CreateStudentCommand(string studentNumber, Guid userId, Guid departmentId, string program, DateTime enrollmentDate)
    {
        StudentNumber = studentNumber;
        UserId = userId;
        DepartmentId = departmentId;
        Program = program;
        EnrollmentDate = enrollmentDate;
    }
}

public class SubmitGradeCommand : IRequest<Result<Guid>>
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseTitle { get; set; }
    public int Credits { get; set; }
    public string Grade { get; set; }
    public string Semester { get; set; }
    public int AcademicYear { get; set; }
    public Guid SubmittedById { get; set; }
    public string Remarks { get; set; }

    public SubmitGradeCommand(Guid studentId, Guid courseId, string courseCode, string courseTitle, int credits, string grade,
        string semester,
        int academicYear,
        Guid submittedById,
        string remarks = null)
    {
        StudentId = studentId;
        CourseId = courseId;
        CourseCode = courseCode;
        CourseTitle = courseTitle;
        Credits = credits;
        Grade = grade;
        Semester = semester;
        AcademicYear = academicYear;
        SubmittedById = submittedById;
        Remarks = remarks;
    }
}

public class UpdateStudentStatusCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }

    public UpdateStudentStatusCommand(Guid studentId, string status, string reason)
    {
        StudentId = studentId;
        Status = status;
        Reason = reason;
    }
}

public class CalculateCGPACommand : IRequest<Result<decimal>>
{
    public Guid StudentId { get; set; }

    public CalculateCGPACommand(Guid studentId)
    {
        StudentId = studentId;
    }
}
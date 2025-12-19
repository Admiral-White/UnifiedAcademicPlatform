using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.StudentAcademic.Domain.Interfaces;

public interface IUnitOfWork : UAP.Shared.Infrastructure.Interfaces.IUnitOfWork
{
    IStudentRepository StudentRepository { get; }
    ICourseGradeRepository CourseGradeRepository { get; }
    IAcademicSemesterRepository AcademicSemesterRepository { get; }
}
using MediatR;
using UAP.Registration.Domain.Enums;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.Commands;

public record RegisterCourseCommand(
    Guid StudentId,
    Guid CourseId,
    string SemesterId,
    int AcademicYear,
    RegistrationType Type = RegistrationType.CourseRegistration
) : IRequest<Result<Guid>>;

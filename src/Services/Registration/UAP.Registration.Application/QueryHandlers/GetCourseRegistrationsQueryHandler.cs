using MediatR;
using UAP.Registration.Application.Queries;
using UAP.Registration.Domain.DTOs;
using UAP.Registration.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.QueryHandlers;

public class GetCourseRegistrationsQueryHandler : IRequestHandler<GetCourseRegistrationsQuery, Result<IEnumerable<CourseRegistrationDto>>>
{
    private readonly ICourseRegistrationRepository _repository;

    public GetCourseRegistrationsQueryHandler(ICourseRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<CourseRegistrationDto>>> Handle(GetCourseRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var registrations = await _repository.GetByCourseIdAsync(request.CourseId, cancellationToken);

        var dtos = registrations.Select(r => new CourseRegistrationDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            CourseId = r.CourseId,
            SemesterId = r.SemesterId,
            AcademicYear = r.AcademicYear,
            Status = r.Status,
            Type = r.Type,
            RegistrationDate = r.RegistrationDate,
            ApprovalDate = r.ApprovalDate,
            ApprovedBy = r.ApprovedBy.ToString(),
            RejectionReason = r.RejectionReason
        });

        return Result.Success(dtos);
    }
}
using MediatR;
using UAP.Registration.Application.Queries;
using UAP.Registration.Domain.DTOs;
using UAP.Registration.Domain.Interfaces;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.QueryHandlers;

public class GetRegistrationByIdQueryHandler : IRequestHandler<GetRegistrationByIdQuery, Result<CourseRegistrationDto>>
{
    private readonly ICourseRegistrationRepository _repository;

    public GetRegistrationByIdQueryHandler(ICourseRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CourseRegistrationDto>> Handle(GetRegistrationByIdQuery request, CancellationToken cancellationToken)
    {
        var registration = await _repository.GetByIdAsync(request.RegistrationId, cancellationToken);
        
        if (registration == null)
            return Result.Failure<CourseRegistrationDto>("Registration not found");

        var dto = new CourseRegistrationDto
        {
            Id = registration.Id,
            StudentId = registration.StudentId,
            CourseId = registration.CourseId,
            SemesterId = registration.SemesterId,
            AcademicYear = registration.AcademicYear,
            Status = registration.Status,
            Type = registration.Type,
            RegistrationDate = registration.RegistrationDate,
            ApprovalDate = registration.ApprovalDate,
            ApprovedBy = registration.ApprovedBy.ToString(),
            RejectionReason = registration.RejectionReason
        };

        return Result.Success(dto);
    }
}
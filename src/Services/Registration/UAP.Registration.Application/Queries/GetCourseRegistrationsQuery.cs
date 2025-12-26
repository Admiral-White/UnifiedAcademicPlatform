using MediatR;
using UAP.Registration.Domain.DTOs;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.Queries;

public record GetCourseRegistrationsQuery(Guid CourseId) : IRequest<Result<IEnumerable<CourseRegistrationDto>>>;
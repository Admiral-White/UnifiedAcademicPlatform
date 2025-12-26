using MediatR;
using UAP.Registration.Domain.DTOs;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.Queries;

public record GetRegistrationByIdQuery(Guid RegistrationId) : IRequest<Result<CourseRegistrationDto>>;
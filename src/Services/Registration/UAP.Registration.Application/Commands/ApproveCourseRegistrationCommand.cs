using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.Commands;

public record ApproveCourseRegistrationCommand(Guid RegistrationId, Guid ApprovedBy) : IRequest<Result<Guid>>;

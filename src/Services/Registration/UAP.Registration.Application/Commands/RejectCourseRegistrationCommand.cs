using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.Registration.Application.Commands;

public record RejectCourseRegistrationCommand(
    Guid RegistrationId,
    string Reason
) : IRequest<Result<Guid>>;
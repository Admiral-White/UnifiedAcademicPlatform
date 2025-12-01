using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Commands;

public class DeactivateCourseCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
    public string Reason { get; set; }

    public DeactivateCourseCommand(Guid courseId, string reason = null)
    {
        CourseId = courseId;
        Reason = reason;
    }
}
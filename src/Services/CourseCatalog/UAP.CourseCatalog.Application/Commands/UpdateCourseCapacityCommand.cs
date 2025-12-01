using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Commands;

public class UpdateCourseCapacityCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
    public int NewCapacity { get; set; }

    public UpdateCourseCapacityCommand(Guid courseId, int newCapacity)
    {
        CourseId = courseId;
        NewCapacity = newCapacity;
    }
}
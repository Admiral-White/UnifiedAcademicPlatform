using MediatR;
using UAP.SharedKernel.Common;
using UAP.CourseCatalog.Application.DTOs;

namespace UAP.CourseCatalog.Application.Queries;

public class GetCourseQuery : IRequest<Result<CourseDto>>
{
    public Guid CourseId { get; set; }

    public GetCourseQuery(Guid courseId)
    {
        CourseId = courseId;
    }
}
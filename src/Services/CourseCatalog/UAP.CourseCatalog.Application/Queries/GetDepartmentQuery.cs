using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Queries;

public class GetDepartmentQuery : IRequest<Result<DepartmentDto>>
{
    public Guid DepartmentId { get; set; }

    public GetDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid HeadOfDepartmentId { get; set; }
    public string HeadOfDepartmentName { get; set; }
    public bool IsActive { get; set; }
    public List<CourseDto> Courses { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
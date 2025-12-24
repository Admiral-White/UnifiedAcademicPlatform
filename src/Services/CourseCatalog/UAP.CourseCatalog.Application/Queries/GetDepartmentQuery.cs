using MediatR;
using UAP.SharedKernel.Common;
using UAP.CourseCatalog.Application.DTOs;

namespace UAP.CourseCatalog.Application.Queries;

public class GetDepartmentQuery : IRequest<Result<DepartmentDto>>
{
    public Guid DepartmentId { get; set; }

    public GetDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}
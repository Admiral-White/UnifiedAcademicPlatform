using MediatR;
using UAP.SharedKernel.Common;
using UAP.CourseCatalog.Application.DTOs;

namespace UAP.CourseCatalog.Application.Queries;

public class GetDepartmentsQuery : IRequest<Result<List<DepartmentDto>>>
{
    public bool IncludeInactive { get; set; } = false;

    public GetDepartmentsQuery(bool includeInactive = false)
    {
        IncludeInactive = includeInactive;
    }
}
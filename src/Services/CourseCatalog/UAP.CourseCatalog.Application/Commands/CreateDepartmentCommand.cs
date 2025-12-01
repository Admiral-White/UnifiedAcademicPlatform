using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Commands;

public class CreateDepartmentCommand : IRequest<Result<Guid>>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid HeadOfDepartmentId { get; set; }

    public CreateDepartmentCommand(
        string code,
        string name,
        string description,
        Guid headOfDepartmentId)
    {
        Code = code;
        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
    }
}
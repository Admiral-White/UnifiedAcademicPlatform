using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Domain.Events;

public class DepartmentDeactivatedDomainEvent : DomainEvent
{
    public Guid DepartmentId { get; }
    public string Code { get; }
    public string Name { get; }

    public DepartmentDeactivatedDomainEvent(Guid departmentId, string code, string name)
    {
        DepartmentId = departmentId;
        Code = code;
        Name = name;
    }
}
namespace UAP.Shared.Contracts.IntegrationEvents;

public class DepartmentCreatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid DepartmentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid HeadOfDepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }

    public DepartmentCreatedIntegrationEvent(
        Guid departmentId,
        string code,
        string name,
        string description,
        Guid headOfDepartmentId,
        DateTime createdAt)
    {
        DepartmentId = departmentId;
        Code = code;
        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
        CreatedAt = createdAt;
    }
}

public class DepartmentUpdatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid DepartmentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid HeadOfDepartmentId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DepartmentUpdatedIntegrationEvent(
        Guid departmentId,
        string code,
        string name,
        string description,
        Guid headOfDepartmentId,
        DateTime updatedAt)
    {
        DepartmentId = departmentId;
        Code = code;
        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
        UpdatedAt = updatedAt;
    }
}

public class DepartmentDeactivatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid DepartmentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DateTime DeactivatedAt { get; set; }

    public DepartmentDeactivatedIntegrationEvent(
        Guid departmentId,
        string code,
        string name,
        DateTime deactivatedAt)
    {
        DepartmentId = departmentId;
        Code = code;
        Name = name;
        DeactivatedAt = deactivatedAt;
    }
}
using UAP.CourseCatalog.Domain.Events;
using UAP.SharedKernel.Common;
using UAP.SharedKernel.Entity;

namespace UAP.CourseCatalog.Domain.Entities;


/*public class Department : AggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid HeadOfDepartmentId { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<Course> _courses = new();
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Department() { }

    public Department(string code, string name, string description, Guid headOfDepartmentId)
    {
        Id = Guid.NewGuid();
        Code = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
        IsActive = true;
    }

    public void UpdateDetails(string name, string description, Guid headOfDepartmentId)
    {
        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
    }

    public void AddCourse(Course course)
    {
        if (_courses.Any(c => c.CourseCode == course.CourseCode))
            throw new InvalidOperationException($"Course with code {course.CourseCode} already exists in department");
            
        _courses.Add(course);
    }

    public void Deactivate()
    {
        IsActive = false;
        // Also deactivate all courses in the department
        foreach (var course in _courses)
        {
            course.Deactivate();
        }
    }
}*/


// using UAP.CourseCatalog.Domain.Events;
// using UAP.SharedKernel.Common;
// using UAP.SharedKernel.Entity;
//
// namespace UAP.CourseCatalog.Domain.Entities;

public class Department : AggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid HeadOfDepartmentId { get; private set; }
    public bool IsActive { get; private set; }
    
    // Audit properties
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    private readonly List<Course> _courses = new();
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Department() { }

    public Department(string code, string name, string description, Guid headOfDepartmentId)
    {
        Id = Guid.NewGuid();
        Code = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
        IsActive = true;
        
        // Set audit fields
        CreatedOn = DateTime.UtcNow;
        CreatedBy = "system";
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = "system";
        
        AddDomainEvent(new DepartmentCreatedDomainEvent(Id, Code, Name));
    }

    public Result UpdateDetails(string name, string description, Guid headOfDepartmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Name cannot be empty");

        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
        
        AddDomainEvent(new DepartmentUpdatedDomainEvent(Id, Code, Name));
        
        return Result.Success();
    }

    public void AddCourse(Course course)
    {
        if (_courses.Any(c => c.CourseCode == course.CourseCode))
            throw new InvalidOperationException($"Course with code {course.CourseCode} already exists in department");
            
        _courses.Add(course);
    }

    public void Deactivate()
    {
        IsActive = false;
        
        // Also deactivate all courses in the department
        foreach (var course in _courses)
        {
            course.Deactivate();
        }
        
        AddDomainEvent(new DepartmentDeactivatedDomainEvent(Id, Code, Name));
    }
}
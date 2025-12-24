using UAP.CourseCatalog.Domain.Enums;
using UAP.CourseCatalog.Domain.Events;
using UAP.SharedKernel.Common;
using UAP.SharedKernel.Entity;

namespace UAP.CourseCatalog.Domain.Entities;

public class Course : AggregateRoot<Guid>
{
    public string CourseCode { get; set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Credits { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid CoordinatorId { get; private set; }
    // FIX: Add the Department navigation property
    public Department Department { get; private set; }
    public int MaxCapacity { get; private set; }
    public int CurrentEnrollment { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsBorrowable { get; private set; }
    public Semester OfferingSemester { get; private set; }
    public int AcademicYear { get; private set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    
    private readonly List<Prerequisite> _prerequisites = new();
    public IReadOnlyCollection<Prerequisite> Prerequisites => _prerequisites.AsReadOnly();

    // Private constructor for EF Core
    private Course() { }

    public Course(
        string courseCode,
        string title,
        string description,
        int credits,
        Guid departmentId,
        Guid coordinatorId,
        int maxCapacity,
        bool isBorrowable,
        Semester offeringSemester,
        int academicYear)
    {
        Id = Guid.NewGuid();
        CourseCode = courseCode?.ToUpper() ?? throw new ArgumentNullException(nameof(courseCode));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        Credits = credits > 0 ? credits : throw new ArgumentException("Credits must be positive", nameof(credits));
        DepartmentId = departmentId;
        CoordinatorId = coordinatorId;
        MaxCapacity = maxCapacity > 0 ? maxCapacity : throw new ArgumentException("Max capacity must be positive", nameof(maxCapacity));
        CurrentEnrollment = 0;
        IsActive = true;
        IsBorrowable = isBorrowable;
        OfferingSemester = offeringSemester;
        AcademicYear = academicYear;
        
        AddDomainEvent(new CourseCreatedDomainEvent(Id, CourseCode, Title, DepartmentId));
    }

    public Result UpdateDetails(string title, string description, int credits)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure("Title cannot be empty");
            
        if (credits <= 0)
            return Result.Failure("Credits must be positive");

        Title = title;
        Description = description;
        Credits = credits;
        
        AddDomainEvent(new CourseUpdatedDomainEvent(Id, CourseCode, Title));
        
        return Result.Success();
    }

    public Result UpdateCapacity(int maxCapacity)
    {
        if (maxCapacity <= 0)
            return Result.Failure("Max capacity must be positive");
            
        if (maxCapacity < CurrentEnrollment)
            return Result.Failure("Max capacity cannot be less than current enrollment");

        MaxCapacity = maxCapacity;
        AddDomainEvent(new CourseCapacityUpdatedDomainEvent(Id, CourseCode, MaxCapacity, CurrentEnrollment));
        
        return Result.Success();
    }

    public Result EnrollStudent()
    {
        if (!IsActive)
            return Result.Failure("Course is not active");
            
        if (CurrentEnrollment >= MaxCapacity)
            return Result.Failure("Course is full");

        CurrentEnrollment++;
        
        if (CurrentEnrollment >= MaxCapacity)
        {
            AddDomainEvent(new CourseFullDomainEvent(Id, CourseCode, Title));
        }
        
        return Result.Success();
    }

    public Result DropStudent()
    {
        if (CurrentEnrollment <= 0)
            return Result.Failure("No students enrolled to drop");

        CurrentEnrollment--;
        return Result.Success();
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new CourseDeactivatedDomainEvent(Id, CourseCode, Title));
    }

    public void AddPrerequisite(Guid prerequisiteCourseId)
    {
        if (_prerequisites.Any(p => p.PrerequisiteCourseId == prerequisiteCourseId))
            return;
            
        _prerequisites.Add(new Prerequisite(Id, prerequisiteCourseId));
    }

    public void RemovePrerequisite(Guid prerequisiteCourseId)
    {
        var prerequisite = _prerequisites.FirstOrDefault(p => p.PrerequisiteCourseId == prerequisiteCourseId);
        if (prerequisite != null)
            _prerequisites.Remove(prerequisite);
    }

    public bool HasPrerequisite(Guid courseId)
    {
        return _prerequisites.Any(p => p.PrerequisiteCourseId == courseId);
    }

    public void UpdateBorrowableStatus(bool isBorrowable)
    {
        IsBorrowable = isBorrowable;
        AddDomainEvent(new CourseUpdatedDomainEvent(Id, CourseCode, Title));
    }
}

public class Prerequisite : Entity<Guid>
{
    public Guid CourseId { get; private set; }
    public Guid PrerequisiteCourseId { get; private set; }

    // Navigation properties
    public Course Course { get; private set; }
    public Course PrerequisiteCourse { get; private set; }

    private Prerequisite() { }

    public Prerequisite(Guid courseId, Guid prerequisiteCourseId)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        PrerequisiteCourseId = prerequisiteCourseId;
    }
}
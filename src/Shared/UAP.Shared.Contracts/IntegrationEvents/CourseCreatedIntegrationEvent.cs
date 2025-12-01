namespace UAP.Shared.Contracts.IntegrationEvents;

public class CourseCreatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid CoordinatorId { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsBorrowable { get; set; }
    public string OfferingSemester { get; set; }
    public int AcademicYear { get; set; }
    public DateTime CreatedAt { get; set; }

    public CourseCreatedIntegrationEvent(
        Guid courseId,
        string courseCode,
        string title,
        string description,
        int credits,
        Guid departmentId,
        Guid coordinatorId,
        int maxCapacity,
        bool isBorrowable,
        string offeringSemester,
        int academicYear,
        DateTime createdAt)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
        Description = description;
        Credits = credits;
        DepartmentId = departmentId;
        CoordinatorId = coordinatorId;
        MaxCapacity = maxCapacity;
        IsBorrowable = isBorrowable;
        OfferingSemester = offeringSemester;
        AcademicYear = academicYear;
        CreatedAt = createdAt;
    }
}

public class CourseUpdatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CourseUpdatedIntegrationEvent(
        Guid courseId,
        string courseCode,
        string title,
        string description,
        int credits,
        DateTime updatedAt)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
        Description = description;
        Credits = credits;
        UpdatedAt = updatedAt;
    }
}

public class CourseCapacityUpdatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public int AvailableSlots { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CourseCapacityUpdatedIntegrationEvent(
        Guid courseId,
        string courseCode,
        int maxCapacity,
        int currentEnrollment,
        DateTime updatedAt)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        MaxCapacity = maxCapacity;
        CurrentEnrollment = currentEnrollment;
        AvailableSlots = maxCapacity - currentEnrollment;
        UpdatedAt = updatedAt;
    }
}

public class CourseFullIntegrationEvent : BaseIntegrationEvent
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public int MaxCapacity { get; set; }
    public DateTime OccurredAt { get; set; }

    public CourseFullIntegrationEvent(
        Guid courseId,
        string courseCode,
        string title,
        int maxCapacity,
        DateTime occurredAt)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
        MaxCapacity = maxCapacity;
        OccurredAt = occurredAt;
    }
}

public class CourseDeactivatedIntegrationEvent : BaseIntegrationEvent
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string Title { get; set; }
    public DateTime DeactivatedAt { get; set; }

    public CourseDeactivatedIntegrationEvent(
        Guid courseId,
        string courseCode,
        string title,
        DateTime deactivatedAt)
    {
        CourseId = courseId;
        CourseCode = courseCode;
        Title = title;
        DeactivatedAt = deactivatedAt;
    }
}
using MediatR;
using UAP.SharedKernel.Common;

namespace UAP.CourseCatalog.Application.Commands;

public class UpdateCourseCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Credits { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsBorrowable { get; set; }

    public UpdateCourseCommand(
        Guid courseId,
        string title,
        string description,
        int credits,
        int maxCapacity,
        bool isBorrowable)
    {
        CourseId = courseId;
        Title = title;
        Description = description;
        Credits = credits;
        MaxCapacity = maxCapacity;
        IsBorrowable = isBorrowable;
    }
}
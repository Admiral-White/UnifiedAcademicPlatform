using UAP.CourseCatalog.Domain.Enums;

namespace UAP.CourseCatalog.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid CoordinatorId { get; set; }
        public string CoordinatorName { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public bool IsBorrowable { get; set; }
        public Semester OfferingSemester { get; set; }
        public int AcademicYear { get; set; }
        public bool IsActive { get; set; }
        public List<PrerequisiteDto> Prerequisites { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
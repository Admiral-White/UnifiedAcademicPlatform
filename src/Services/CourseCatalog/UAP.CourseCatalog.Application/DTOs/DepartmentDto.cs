namespace UAP.CourseCatalog.Application.DTOs
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid HeadOfDepartmentId { get; set; }
        public string HeadOfDepartmentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CourseCount { get; set; }
        public List<CourseDto> Courses { get; set; } = new();
    }
}
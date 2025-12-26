namespace UAP.Shared.Contracts.IntegrationEvents;

public class GradeSubmittedIntegrationEvent : BaseIntegrationEvent
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Grade { get; set; }
    public string Semester { get; set; }
    public int AcademicYear { get; set; }
    public DateTime SubmittedAt { get; set; }

    public GradeSubmittedIntegrationEvent(Guid studentId, Guid courseId, string grade, string semester, int academicYear, DateTime submittedAt)
    {
        StudentId = studentId;
        CourseId = courseId;
        Grade = grade;
        Semester = semester;
        AcademicYear = academicYear;
        SubmittedAt = submittedAt;
    }
}

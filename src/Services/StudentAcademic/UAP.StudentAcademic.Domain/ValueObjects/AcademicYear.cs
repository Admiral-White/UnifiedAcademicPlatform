using UAP.SharedKernel.Common;
using UAP.StudentAcademic.Domain.Enums;

namespace UAP.StudentAcademic.Domain.ValueObjects;

public class AcademicYear : ValueObject
{
    public int Year { get; }
    public SemesterType Semester { get; }

    public AcademicYear(int year, SemesterType semester)
    {
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Year must be between 2000 and 2100", nameof(year));

        Year = year;
        Semester = semester;
    }

    public string Display => $"{Semester} {Year}";

    public AcademicYear NextSemester()
    {
        return Semester switch
        {
            SemesterType.Fall => new AcademicYear(Year + 1, SemesterType.Spring),
            SemesterType.Spring => new AcademicYear(Year, SemesterType.Summer),
            SemesterType.Summer => new AcademicYear(Year, SemesterType.Fall),
            SemesterType.Winter => new AcademicYear(Year, SemesterType.Spring),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool IsCurrent()
    {
        var currentDate = DateTime.UtcNow;
        var currentYear = currentDate.Year;
        var currentMonth = currentDate.Month;

        return (Semester, currentMonth) switch
        {
            (SemesterType.Fall, >= 8 and <= 12) when Year == currentYear => true,
            (SemesterType.Spring, >= 1 and <= 5) when Year == currentYear => true,
            (SemesterType.Summer, >= 5 and <= 8) when Year == currentYear => true,
            (SemesterType.Winter, 12 or 1) when Year == currentYear || Year == currentYear - 1 => true,
            _ => false
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Year;
        yield return Semester;
    }
}
namespace UAP.StudentAcademic.Domain.Enums;

public enum StudentStatus
{
    Active = 1,
    Probation = 2,
    Suspended = 3,
    Graduated = 4,
    Withdrawn = 5,
    LeaveOfAbsence = 6
}

public enum Grade
{
    A = 1,      // 4.0
    BPlus = 2,  // 3.5
    B = 3,      // 3.0
    CPlus = 4,  // 2.5
    C = 5,      // 2.0
    D = 6,      // 1.0
    F = 7,      // 0.0
    W = 8,      // Withdrawn
    I = 9,      // Incomplete
    P = 10      // Pass (for pass/fail courses)
}

public enum SemesterType
{
    Fall = 1,
    Spring = 2,
    Summer = 3,
    Winter = 4
}

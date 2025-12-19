using UAP.SharedKernel.Common;

namespace UAP.StudentAcademic.Domain.ValueObjects;

public class CGPA : ValueObject
{
    public decimal Value { get; }
    public decimal Scale { get; } = 4.0m; // 4.0 scale

    private CGPA() { } // For EF Core

    private CGPA(decimal value)
    {
        if (value < 0 || value > Scale)
            throw new ArgumentException($"CGPA must be between 0 and {Scale}", nameof(value));

        Value = Math.Round(value, 2); // Round to 2 decimal places
    }

    public static Result<CGPA> Create(decimal value)
    {
        if (value < 0 || value > 4.0m)
            return Result.Failure<CGPA>($"CGPA must be between 0 and 4.0");

        return Result.Success(new CGPA(value));
    }

    public static CGPA Zero => new CGPA(0.0m);

    public override string ToString() => Value.ToString("0.00");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    // Business rules
    public bool IsGoodStanding() => Value >= 2.0m;
    public bool IsDeanList() => Value >= 3.5m;
    public bool IsProbation() => Value < 2.0m && Value >= 1.0m;
    public bool IsDismissal() => Value < 1.0m;
}
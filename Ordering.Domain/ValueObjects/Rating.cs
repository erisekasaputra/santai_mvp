using Ordering.Domain.SeedWork;

namespace Ordering.Domain.ValueObjects;

public class Rating : ValueObject
{
    public decimal Value { get; private set; }
    public string? Comment { get; private set; }

    public Rating(decimal value, string? comment)
    {
        Value = value;
        Comment = comment;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Comment;
    }
}

using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.OwnerReviewAggregate;

public class OwnerReview : ValueObject
{ 
    public string Title { get; private set; }
    public int Rating { get; private set; } 
     
    public OwnerReview(string title, int rating)
    {
        Title = title; 
        Rating = (rating is < 0 or > 10) ? throw new ArgumentOutOfRangeException(nameof(rating)) : rating; 
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Rating;
    }
}

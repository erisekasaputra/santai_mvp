using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.OwnerReviewAggregate;

public class OwnerReview : Entity
{ 
    public string Title { get; private set; }
    public int Rating { get; private set; }
    public string ItemId { get; private set; }
    public Item Item { get; private set; }

    public OwnerReview()
    {

    }
    public OwnerReview(string title, int rating, string itemId, Item item)
    {
        Title = title; 
        Rating = (rating is < 0 or > 10) ? throw new ArgumentOutOfRangeException(nameof(rating)) : rating;
        ItemId = itemId;
        Item = item;
    }
}

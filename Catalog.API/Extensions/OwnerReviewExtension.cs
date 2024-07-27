using Catalog.API.DTOs.OwnerReviewDto;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;

namespace Catalog.API.Extensions;

public static class OwnerReviewExtension
{
    public static IEnumerable<OwnerReview> ToOwnerReviews(this IEnumerable<OwnerReviewDto> ownerReviewsDto)
    { 
        foreach(var ownerReview in ownerReviewsDto)
        {
            yield return ownerReview.ToOwnerReview();
        }
    }
    public static OwnerReview ToOwnerReview(this OwnerReviewDto ownerReview) 
    {
        return new OwnerReview(ownerReview.Title, ownerReview.Rating);
    }
}

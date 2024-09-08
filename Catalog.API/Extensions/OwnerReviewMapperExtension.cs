
using Catalog.API.Applications.Dtos.OwnerReview;
using Catalog.Domain.Aggregates.OwnerReviewAggregate;

namespace Catalog.API.Extensions;

public static class OwnerReviewMapperExtension
{
    public static IEnumerable<OwnerReview?> ToOwnerReviews(this IEnumerable<OwnerReviewDto> ownerReviewsDto)
    { 
        if (ownerReviewsDto is null)
        { 
            yield break;
        }

        foreach(var ownerReview in ownerReviewsDto!)
        {
            yield return ownerReview.ToOwnerReview();
        }
    }
    public static OwnerReview? ToOwnerReview(this OwnerReviewDto ownerReview) 
    {
        if (ownerReview is null)
        {
            return null;
        }

        return new OwnerReview(ownerReview.Title, ownerReview.Rating);
    }    


    
    public static IEnumerable<OwnerReviewDto?> ToOwnerReviewsDto(this IEnumerable<OwnerReview> ownerReviewsDto)
    { 
        if (ownerReviewsDto is null)
        { 
            yield break;
        }

        foreach(var ownerReview in ownerReviewsDto!)
        {
            yield return ownerReview.ToOwnerReviewDto();
        }
    }
    public static OwnerReviewDto? ToOwnerReviewDto(this OwnerReview ownerReview) 
    {
        if (ownerReview is null)
        {
            return null;
        }

        return new OwnerReviewDto(ownerReview.Title, ownerReview.Rating);
    }
}

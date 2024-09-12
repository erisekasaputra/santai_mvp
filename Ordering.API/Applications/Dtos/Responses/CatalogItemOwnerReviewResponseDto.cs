namespace Ordering.API.Applications.Dtos.Responses;

public class CatalogItemOwnerReviewResponseDto
{
    public string Title { get; set; }
    public decimal Rating { get; set; }

    public CatalogItemOwnerReviewResponseDto(
        string title,
        decimal rating)
    {
        Title = title;
        Rating = rating;
    }
}

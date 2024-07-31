using Catalog.API.DTOs.OwnerReview; 

namespace Catalog.API.DTOs.Item;

public record ItemDto(string Id,
                  string Name,
                  string Description,
                  decimal Price,
                  string ImageUrl,
                  DateTime CreatedAt,
                  int StockQuantity,
                  int SoldQuantity,
                  string CategoryId,
                  string CategoryName,
                  string BrandId,
                  string BrandName,
                  IEnumerable<OwnerReviewDto>? OwnerReviews);
using Catalog.API.Applications.Dtos.OwnerReview;
using Core.Enumerations;
using System.ComponentModel.DataAnnotations;
namespace Catalog.API.Applications.Dtos.Item;

public record ItemDto(
    Guid Id,
    [Required] string Name,
    [Required] string Description,
    [Required] string Sku,
    [Required] decimal Price,
    [Required] Currency Currency,
    [Required] string ImageUrl,
    [Required] DateTime CreatedAt,
    [Required] int StockQuantity,
    [Required] int SoldQuantity,
    Guid? CategoryId,
    string CategoryName,
    Guid? BrandId,
    string BrandName,
    [Required] bool IsActive,
    IEnumerable<OwnerReviewDto>? OwnerReviews);
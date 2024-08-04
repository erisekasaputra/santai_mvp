using Catalog.API.DTOs.Item;
using Catalog.API.Extensions;
using Catalog.API.SeedWork;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.CreateItem;

public class CreateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateItemCommand, Result<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<ItemDto>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return Result<ItemDto>.Failure($"Category with id {request.CategoryId} not found.", 404);
        }

        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.BrandId);
        if (brand is null)
        {
            return Result<ItemDto>.Failure($"Brand with id {request.BrandId} not found.", 404);
        }

        var ownerReviews = request.OwnerReviews.ToOwnerReviews().ToList();

        var item = new Item(
               request.Name,
               request.Description,
               request.Price,
               request.ImageUrl,
               DateTime.UtcNow,
               request.StockQuantity,
               request.SoldQuantity,
               category.Id,
               category,
               brand.Id,
               brand,
               request.IsActive,
               ownerReviews!
           );

        var response = await _unitOfWork.Items.CreateItemAsync(item);
        if (response is null)
        {
            return Result<ItemDto>.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", 500);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result <= 0)
        {
            return Result<ItemDto>.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", 500);
        }

        var responseDto = new ItemDto(
            response.Id,
            response.Name,
            response.Description,
            response.Price,
            response.ImageUrl,
            DateTime.UtcNow,
            response.StockQuantity,
            response.SoldQuantity,
            response.Category?.Id ?? string.Empty,
            response.Category?.Name ?? string.Empty,
            response.Brand?.Id ?? string.Empty,
            response.Brand?.Name ?? string.Empty,
            response.IsActive,
            response.OwnerReviews.ToOwnerReviewsDto()!);

        return Result<ItemDto>.SuccessResult(responseDto, [], 201);
    }
}

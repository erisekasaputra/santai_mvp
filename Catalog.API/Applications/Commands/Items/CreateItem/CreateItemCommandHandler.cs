using Catalog.API.Applications.Dtos.Item;
using Catalog.API.Extensions; 
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR; 

namespace Catalog.API.Applications.Commands.Items.CreateItem;

public class CreateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return Result.Failure($"Category with id {request.CategoryId} not found.", ResponseStatus.NotFound);
        }

        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.BrandId);
        if (brand is null)
        {
            return Result.Failure($"Brand with id {request.BrandId} not found.", ResponseStatus.NotFound);
        }

        var ownerReviews = request.OwnerReviews.ToOwnerReviews().ToList();

        var item = new Item(
               request.Name,
               request.Description,
               request.Price,
               request.Currency,
               request.Sku,
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
            return Result.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result <= 0)
        {
            return Result.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
        }

        var responseDto = new ItemDto(
            response.Id,
            response.Name,
            response.Description,
            response.Sku,
            response.Price.Amount,
            response.Price.Currency,
            response.ImageUrl,
            DateTime.UtcNow,
            response.StockQuantity,
            response.SoldQuantity,
            response.Category?.Id,
            response.Category?.Name ?? string.Empty,
            response.Brand?.Id,
            response.Brand?.Name ?? string.Empty,
            response.IsActive,
            response.OwnerReviews.ToOwnerReviewsDto()!);

        return Result.Success(responseDto, ResponseStatus.Created);
    }
}

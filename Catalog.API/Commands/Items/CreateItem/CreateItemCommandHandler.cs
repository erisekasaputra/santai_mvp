using Catalog.API.SeedWorks;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Commands.Items.CreateItem;

public class CreateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateItemCommand, Result<CreateItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<CreateItemResponse>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId);

        if (category is null)
        {
            return Result<CreateItemResponse>.Failure($"Category with id {request.CategoryId} not found.", 404);
        }

        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.BrandId);

        if (brand is null)
        {
            return Result<CreateItemResponse>.Failure($"Brand with id {request.BrandId} not found.", 404);
        }

        var ownerReviews = request.OwnerReviews.ToList() ?? throw new ArgumentException("Owner reviews can not be null, please provide at least one Key Value pair e.g: [ Performance : 9.3 ]");
       
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
               ownerReviews
           );

        var response = await _unitOfWork.Items.CreateItemAsync(item);

        if (response is null)
        {
            return Result<CreateItemResponse>.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", 500);
        }
         
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            return Result<CreateItemResponse>.Failure("We encountered an issue while creating the item. Please try again later or contact support if the problem persists.", 500);
        }

        return Result<CreateItemResponse>.SuccessResult(
                CreateItemResponse.Create(
                        response.Id,
                        response.Name,
                        response.Description,
                        response.Price,
                        response.ImageUrl,
                        DateTime.UtcNow,
                        response.StockQuantity,
                        response.SoldQuantity,
                        response.CategoryId,
                        response.Category.Name,
                        response.Brand.Id,
                        response.Brand.Name
                    ), [], 201
            );
    }
}

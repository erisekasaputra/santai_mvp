using Catalog.API.Extensions;
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Commands.Items.UpdateItem;

public class UpdateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateItemCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId); 
        if (category is null)
        {
            return Result<Unit>.Failure($"Category with id {request.CategoryId} is not found", 404);
        }

        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.BrandId); 
        if (brand is null)
        {
            return Result<Unit>.Failure($"Brand with id {request.BrandId} is not found", 404);
        }

        var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id); 
        if (item is null)
        {
            return Result<Unit>.Failure($"Item with id {request.Id} is not found", 404);
        } 

        var ownerReviews = request.OwnerReviews.ToOwnerReviews().ToList(); 
        if (ownerReviews.Count <= 0)
        {
            return Result<Unit>.Failure("Owner reviews can not be null, please fill at least one key value pair, e.g : [Performance: 9.2]");
        }

        item.Update(request.Name, request.Description, request.ImageUrl, category.Id, category, brand.Id, brand, ownerReviews);
        item.SetPrice(request.Price);
        item.SetStockQuantity(request.StockQuantity);
        item.SetSoldQuantity(request.SoldQuantity);

        _unitOfWork.Items.UpdateItem(item);  
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

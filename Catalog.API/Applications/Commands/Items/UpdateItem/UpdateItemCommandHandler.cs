using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.UpdateItem;

public class UpdateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
            {
                return Result.Failure($"Category with id {request.CategoryId} is not found", ResponseStatus.NotFound);
            }

            var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.BrandId);
            if (brand is null)
            {
                return Result.Failure($"Brand with id {request.BrandId} is not found", ResponseStatus.NotFound);
            }

            var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id);
            if (item is null)
            {
                return Result.Failure($"Item with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            if (item.IsDeleted)
            {
                return Result.Failure($"Item with id {request.Id} is not found or it has been deleted", ResponseStatus.NotFound);
            }

            var ownerReviews = request.OwnerReviews.ToOwnerReviews().ToList();

            item.Update(
                request.Name,
                request.Description,
                request.Sku,
                request.ImageUrl,
                category.Id,
                category,
                brand.Id,
                brand,
                request.IsActive,
                ownerReviews!,
                request.Price,
                request.Currency,
                request.StockQuantity,
                request.SoldQuantity);

            _unitOfWork.Items.UpdateItem(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(Unit.Value, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception)
        {
            return Result.Failure(Messages.InternalServerError, ResponseStatus.BadRequest);
        } 
    }
}

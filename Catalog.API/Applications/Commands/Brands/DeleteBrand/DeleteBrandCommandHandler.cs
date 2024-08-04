using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.DeleteBrand;

public class DeleteBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteBrandCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

        if (item is not null)
        {
            item.Delete();

            _unitOfWork.Brands.UpdateBrand(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

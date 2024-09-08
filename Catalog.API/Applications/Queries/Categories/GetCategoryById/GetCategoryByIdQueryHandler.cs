 
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryByIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Categories.GetCategoryByIdAsync(request.Id);

        if (result is null)
        {
            return Result.Failure($"Category with id {request.Id} not found", ResponseStatus.NotFound);
        }

        return Result.Success(result.ToCategoryDto(), ResponseStatus.Ok);
    }
}

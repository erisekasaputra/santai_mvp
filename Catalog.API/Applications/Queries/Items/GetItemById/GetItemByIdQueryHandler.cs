 
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemById;

public class GetItemByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetItemByIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Items.RetrieveItemById(request.Id);

        if (result is null)
        {
            return Result.Failure($"Product with id {request.Id} not found", ResponseStatus.NotFound);
        }

        return Result.Success(result.ToItemDto(), ResponseStatus.Created);
    }
}

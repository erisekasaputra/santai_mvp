using Catalog.API.DTOs.ItemSold;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceSold;

public class ReduceSoldCommandHandler : IRequestHandler<ReduceSoldCommand, Result<IEnumerable<ItemSoldDto>>>
{
    public Task<Result<IEnumerable<ItemSoldDto>>> Handle(ReduceSoldCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

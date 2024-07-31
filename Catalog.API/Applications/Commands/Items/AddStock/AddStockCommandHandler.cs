
using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddStock;

public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Result<IEnumerable<ItemStockDto>>>
{
    public Task<Result<IEnumerable<ItemStockDto>>> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
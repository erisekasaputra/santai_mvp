using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetStock;

public class SetStockCommandHandler : IRequestHandler<SetStockCommand, Result<IEnumerable<ItemStockDto>>>
{
    public Task<Result<IEnumerable<ItemStockDto>>> Handle(SetStockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

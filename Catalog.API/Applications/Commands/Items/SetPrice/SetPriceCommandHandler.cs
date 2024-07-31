
using Catalog.API.DTOs.ItemPrice;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetPrice;

public class SetPriceCommandHandler : IRequestHandler<SetPriceCommand, Result<IEnumerable<ItemPriceDto>>>
{
    public Task<Result<IEnumerable<ItemPriceDto>>> Handle(SetPriceCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

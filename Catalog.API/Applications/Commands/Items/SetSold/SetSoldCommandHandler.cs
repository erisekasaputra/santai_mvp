using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetSold;

public class SetSoldCommandHandler : IRequestHandler<SetSoldCommand, Result<IEnumerable<ItemSoldDto>>>
{
    public Task<Result<IEnumerable<ItemSoldDto>>> Handle(SetSoldCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

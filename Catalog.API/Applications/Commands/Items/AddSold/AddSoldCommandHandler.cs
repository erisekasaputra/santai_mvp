using Catalog.API.DTOs.ItemSold;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddSold;

public class AddSoldCommandHandler : IRequestHandler<AddSoldCommand, Result<IEnumerable<ItemSoldDto>>>
{
    public Task<Result<IEnumerable<ItemSoldDto>>> Handle(AddSoldCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

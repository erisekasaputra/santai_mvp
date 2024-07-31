using MediatR;

namespace Search.Worker.Applications.Commands.AddStock;

internal class AddStockCommandHandler : IRequestHandler<AddStockCommand, Unit>
{
    public Task<Unit> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using MediatR;

namespace Search.Worker.Applications.Commands.SetPrice;

internal record SetPriceCommand(string Id, decimal Price) : IRequest<Unit>;

using MediatR;

namespace Search.Worker.Applications.Commands.SetItemPrice;

internal record SetItemPriceCommand(string Id, decimal Price) : IRequest<Unit>;

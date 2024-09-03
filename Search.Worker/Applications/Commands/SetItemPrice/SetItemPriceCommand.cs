using MediatR;

namespace Search.Worker.Applications.Commands.SetItemPrice;

internal record SetItemPriceCommand(Guid Id, decimal Price) : IRequest<Unit>;

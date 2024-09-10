using Core.Enumerations;
using MediatR;

namespace Search.Worker.Applications.Commands.SetItemPrice;

internal record SetItemPriceCommand(Guid Id, decimal Price, Currency Currency) : IRequest<Unit>;

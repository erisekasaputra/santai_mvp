using Core.Enumerations;
using MediatR;

namespace Search.Worker.Applications.Commands.SetItemPrice;

internal record SetItemPriceCommand(
    Guid Id, 
    decimal OldPrice, 
    decimal NewPrice, 
    Currency Currency) : IRequest<Unit>;

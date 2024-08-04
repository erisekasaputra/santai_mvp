using MediatR;

namespace Search.Worker.Applications.Commands.SetItemSoldQuantity;

internal record SetItemSoldQuantityCommand(string Id, int Quantity) : IRequest<Unit>;
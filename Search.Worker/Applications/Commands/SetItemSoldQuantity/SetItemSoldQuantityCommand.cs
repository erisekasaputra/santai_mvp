using MediatR;

namespace Search.Worker.Applications.Commands.SetItemSoldQuantity;

internal record SetItemSoldQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>;
using MediatR;

namespace Search.Worker.Applications.Commands.AddItemSoldQuantity;

internal record AddItemSoldQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>;

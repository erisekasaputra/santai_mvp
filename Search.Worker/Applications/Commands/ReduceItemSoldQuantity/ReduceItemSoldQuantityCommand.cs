using MediatR;

namespace Search.Worker.Applications.Commands.ReduceItemSoldQuantity;

internal record ReduceItemSoldQuantityCommand(Guid Id, int Quantity) : IRequest<Unit>;
using MediatR;

namespace Search.Worker.Applications.Commands.ReduceItemSoldQuantity;

internal record ReduceItemSoldQuantityCommand(string Id, int Quantity) : IRequest<Unit>;
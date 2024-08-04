using MediatR;

namespace Search.Worker.Applications.Commands.AddItemSoldQuantity;

internal record AddItemSoldQuantityCommand(string Id, int Quantity) : IRequest<Unit>;

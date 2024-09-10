using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddItemStockQuantity;

public record AddItemStockQuantityCommand(
    IEnumerable<AddItemStockQuantityRequest> Items) : IRequest<Result>;
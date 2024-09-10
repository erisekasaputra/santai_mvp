using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;

public record ReduceItemStockQuantityCommand(
    IEnumerable<ReduceItemStockQuantityRequest> Items) : IRequest<Result>;

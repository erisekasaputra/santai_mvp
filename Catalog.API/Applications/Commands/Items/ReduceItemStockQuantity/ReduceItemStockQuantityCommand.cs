using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceItemStockQuantity;

public record ReduceItemStockQuantityCommand(
    IEnumerable<ReduceItemStockQuantityRequest> ItemIds) : IRequest<Result<IEnumerable<ItemStockDto>>>;

using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceStock;

public record ReduceStockCommand(IEnumerable<ItemStockDto> ItemStocks) : IRequest<Result<IEnumerable<ItemStockDto>>>;

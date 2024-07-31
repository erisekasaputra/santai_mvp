using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetStock;

public record SetStockCommand(IEnumerable<ItemStockDto> ItemStocks) : IRequest<Result<IEnumerable<ItemStockDto>>>;

using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddStock;

public record AddStockCommand(IEnumerable<ItemStockDto> ItemStocks) : IRequest<Result<IEnumerable<ItemStockDto>>>;
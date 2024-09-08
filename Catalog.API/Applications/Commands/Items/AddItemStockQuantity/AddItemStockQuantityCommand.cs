using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddItemStockQuantity;

public record AddItemStockQuantityCommand(
    IEnumerable<AddItemStockQuantityRequest> ItemIds) : IRequest<Result<IEnumerable<ItemStockDto>>>;
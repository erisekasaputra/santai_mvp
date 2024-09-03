using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddItemStockQuantity;

public record AddItemStockQuantityCommand(
    IEnumerable<AddItemStockQuantityRequest> AddItemStockQuantityRequests) : IRequest<Result<IEnumerable<ItemStockDto>>>;
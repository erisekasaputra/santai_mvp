using Catalog.API.DTOs.ItemStock;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemStockQuantity;

public record SetItemStockQuantityCommand(IEnumerable<SetItemStockQuantityRequest> SetItemStockQuantityRequests) : IRequest<Result<IEnumerable<ItemStockDto>>>;

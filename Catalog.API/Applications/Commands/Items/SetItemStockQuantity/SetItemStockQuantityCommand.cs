
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemStockQuantity;

public record SetItemStockQuantityCommand(IEnumerable<SetItemStockQuantityRequest> ItemIds) : IRequest<Result>;

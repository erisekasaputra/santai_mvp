using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;

public record AddItemSoldQuantityCommand(IEnumerable<AddItemSoldQuantityRequest> Items) : IRequest<Result>;

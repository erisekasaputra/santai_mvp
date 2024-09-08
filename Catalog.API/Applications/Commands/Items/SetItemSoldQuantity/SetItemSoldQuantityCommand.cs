using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;

public record SetItemSoldQuantityCommand(
    IEnumerable<SetItemSoldQuantityRequest> ItemIds) : IRequest<Result>;

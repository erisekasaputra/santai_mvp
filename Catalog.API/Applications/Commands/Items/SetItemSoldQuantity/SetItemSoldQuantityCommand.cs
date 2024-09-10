using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;

public record SetItemSoldQuantityCommand(
    IEnumerable<SetItemSoldQuantityRequest> Items) : IRequest<Result>;

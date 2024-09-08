
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemPrice;

public record SetItemPriceCommand(
    IEnumerable<SetItemPriceRequest> ItemIds) : IRequest<Result>;

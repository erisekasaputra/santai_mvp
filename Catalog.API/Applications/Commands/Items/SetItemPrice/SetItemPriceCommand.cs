using Catalog.API.DTOs.ItemPrice; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemPrice;

public record SetItemPriceCommand(
    IEnumerable<SetItemPriceRequest> SetItemPriceRequests) : IRequest<Result<IEnumerable<ItemPriceDto>>>;

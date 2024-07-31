using Catalog.API.DTOs.ItemPrice; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetPrice;

public record SetPriceCommand(IEnumerable<ItemPriceDto> ItemPrices) : IRequest<Result<IEnumerable<ItemPriceDto>>>;

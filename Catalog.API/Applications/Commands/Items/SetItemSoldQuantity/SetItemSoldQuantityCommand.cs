using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetItemSoldQuantity;

public record SetItemSoldQuantityCommand(
    IEnumerable<SetItemSoldQuantityRequest> ItemIds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

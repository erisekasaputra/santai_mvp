using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddItemSoldQuantity;

public record AddItemSoldQuantityCommand(IEnumerable<AddItemSoldQuantityRequest> ItemIds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;

public record ReduceItemSoldQuantityCommand(
    IEnumerable<ReduceItemSoldQuantityRequest> ItemIds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

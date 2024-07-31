using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceSold;

public record ReduceSoldCommand(IEnumerable<ItemSoldDto> ItemSolds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

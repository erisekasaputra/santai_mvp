using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.SetSold;

public record SetSoldCommand(IEnumerable<ItemSoldDto> ItemSolds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

using Catalog.API.DTOs.ItemSold; 
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.AddSold;

public record AddSoldCommand(IEnumerable<ItemSoldDto> ItemSolds) : IRequest<Result<IEnumerable<ItemSoldDto>>>;

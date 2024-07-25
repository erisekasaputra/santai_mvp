using Catalog.API.DTOs.ItemDto;
using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Queries.Items.GetItemById;

public record GetItemByIdQuery(string Id) : IRequest<Result<ItemDto>>;
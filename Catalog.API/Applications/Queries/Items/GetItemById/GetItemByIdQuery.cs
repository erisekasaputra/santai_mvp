using Catalog.API.DTOs.Item;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemById;

public record GetItemByIdQuery(string Id) : IRequest<Result<ItemDto>>;
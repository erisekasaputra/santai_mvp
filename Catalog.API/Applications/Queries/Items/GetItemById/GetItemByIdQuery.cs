using Catalog.API.DTOs.Item;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemById;

public record GetItemByIdQuery(Guid Id) : IRequest<Result<ItemDto>>;
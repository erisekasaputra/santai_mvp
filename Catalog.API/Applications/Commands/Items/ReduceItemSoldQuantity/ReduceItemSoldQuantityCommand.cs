using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ReduceItemSoldQuantity;

public record ReduceItemSoldQuantityCommand(
    IEnumerable<ReduceItemSoldQuantityRequest> ItemIds) : IRequest<Result>;

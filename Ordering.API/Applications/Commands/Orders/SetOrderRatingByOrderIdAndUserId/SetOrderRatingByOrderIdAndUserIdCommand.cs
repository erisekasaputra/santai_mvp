using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetOrderRatingByOrderIdAndUserId;

public record SetOrderRatingByOrderIdAndUserIdCommand(
    Guid OrderId,
    Guid BuyerId,
    decimal Rating,
    string Comment,
    IEnumerable<string> Images) : IRequest<Result>;

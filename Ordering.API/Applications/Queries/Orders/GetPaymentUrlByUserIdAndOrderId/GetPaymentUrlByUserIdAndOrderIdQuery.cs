using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Orders.GetPaymentUrlByUserIdAndOrderId;

public record GetPaymentUrlByUserIdAndOrderIdQuery(Guid OrderId, Guid BuyerId) : IRequest<Result>;

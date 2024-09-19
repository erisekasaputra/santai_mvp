using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.PayCancellationRefundByOrderId;

public record PayCancellationRefundByOrderIdCommand(
    Guid OrderId,
    decimal Amount, 
    Currency Currency) : IRequest<Result>;

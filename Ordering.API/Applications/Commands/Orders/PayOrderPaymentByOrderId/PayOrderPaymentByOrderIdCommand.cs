using Core.Results;
using MediatR; 

namespace Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;

public record PayOrderPaymentByOrderIdCommand(
    Guid OrderId,
    decimal Amount,
    string Method,
    string TransactionId,
    string Message,
    string Hash,
    int Status) : IRequest<Result>;

using Core.Results;
using MediatR; 

namespace Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;

public record PayOrderPaymentByOrderIdCommand(
    Guid OrderId,
    string Name,
    string? Email,
    string? Phone,
    decimal Amount,
    string Method,
    string Reference,
    string Message,
    string Hash,
    int Status) : IRequest<Result>;

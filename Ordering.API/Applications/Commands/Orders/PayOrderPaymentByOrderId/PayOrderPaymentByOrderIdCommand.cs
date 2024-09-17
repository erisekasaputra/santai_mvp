using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;

public record PayOrderPaymentByOrderIdCommand(
    Guid OrderId,
    decimal Amount,
    Currency Currency,
    DateTime PaidAt,
    string? PaymentMethod,
    string? BankReference) : IRequest<Result>;

using Core.Enumerations;
using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.PayOrder;

public record PayOrderCommand(
    Guid OrderId, 
    decimal Amount,
    Currency Currency,
    DateTime PaidAt,
    string? PaymentMethod,
    string? BankReference) : IRequest<Result>;

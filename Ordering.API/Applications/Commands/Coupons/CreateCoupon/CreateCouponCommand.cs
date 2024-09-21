using Core.Enumerations;
using Core.Results;
using MediatR; 
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Commands.Coupons.CreateCoupon;

public record CreateCouponCommand(
    string CouponCode,
    PercentageOrValueType Parameter,
    Currency Currency,
    decimal Value,
    decimal MinimumOrderValue) : IRequest<Result>; 
using Core.Enumerations;
using Core.Results;
using MediatR;
using Ordering.Domain.Enumerations;

namespace Ordering.API.Applications.Commands.Coupons.UpdateCouponById;

public record UpdateCouponByIdCommand(
    Guid CouponId,  
    PercentageOrValueType Parameter,
    Currency Currency,
    decimal Value,
    decimal MinimumOrderValue) : IRequest<Result>;

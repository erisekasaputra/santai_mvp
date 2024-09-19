using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Coupons.DeleteCouponById;

public record DeleteCouponByIdCommand(Guid CouponId) : IRequest<Result>;

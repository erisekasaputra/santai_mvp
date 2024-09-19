using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Coupons.GetCouponById;

public record GetCouponByIdQuery(Guid CouponId) : IRequest<Result>;

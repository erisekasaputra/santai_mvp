using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Coupons.GetCouponByCode;

public record GetCouponByCodeQuery(string Code) : IRequest<Result>;

using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Queries.Coupons.GetPaginatedCoupons;

public record GetPaginatedCouponsQuery(int PageNumber, int PageSize) : IRequest<Result>;

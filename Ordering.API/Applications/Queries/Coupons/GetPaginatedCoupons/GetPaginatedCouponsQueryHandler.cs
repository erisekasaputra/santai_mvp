using Core.Dtos;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses; 
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Coupons.GetPaginatedCoupons;

public class GetPaginatedCouponsQueryHandler(
    ILogger<GetPaginatedCouponsQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetPaginatedCouponsQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetPaginatedCouponsQueryHandler> _logger = logger;
    public async Task<Result> Handle(GetPaginatedCouponsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            (var count, var pages, var items) = await _unitOfWork.Coupons.GetPaginatedCoupons(request.PageNumber, request.PageSize);
            if (items is null || !items.Any())
            {
                return Result.Failure("Coupon is not found", ResponseStatus.NotFound);
            }

            var response = new List<DiscountResponseDto>();

            foreach(var coupon in items)
            {
                response.Add(new DiscountResponseDto(
                    coupon.CouponCode,
                    coupon.CouponValueType,
                    coupon.Currency,
                    coupon.ValuePercentage,
                    coupon.ValueAmount,
                    coupon.MinimumOrderValue,
                    0));
            }

            return Result.Success(new PaginatedResponseDto<DiscountResponseDto>(request.PageNumber, request.PageSize, count, pages, response)); 
        }
        catch (ArgumentNullException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured , Error: {error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

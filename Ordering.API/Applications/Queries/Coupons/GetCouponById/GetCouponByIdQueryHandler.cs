using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Coupons.GetCouponById;

public class GetCouponByIdQueryHandler(
    ILogger<GetCouponByIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetCouponByIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetCouponByIdQueryHandler> _logger = logger;
    public async Task<Result> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.CouponId);
            if (coupon is null)
            {
                return Result.Failure("Coupon is not found", ResponseStatus.NotFound);
            }

            return Result.Success(new DiscountResponseDto(
                coupon.Id,
                coupon.CouponCode,
                coupon.CouponValueType,
                coupon.Currency,
                coupon.ValuePercentage,
                coupon.ValueAmount,
                coupon.MinimumOrderValue,
                0), ResponseStatus.Ok);
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

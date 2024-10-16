using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR; 
using Ordering.API.Applications.Dtos.Responses;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Coupons.GetCouponByCode;

public class GetCouponByCodeQueryHandler(
    ILogger<GetCouponByCodeQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetCouponByCodeQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetCouponByCodeQueryHandler> _logger = logger;
    public async Task<Result> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
    { 
        try
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code);
            if (coupon is null)
            { 
                return Result.Failure("Coupon is not found", ResponseStatus.NotFound);
            } 

            return Result.Success(new DiscountResponseDto(
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
            return Result.Failure(ex.Message, ResponseStatus.NoContent);
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

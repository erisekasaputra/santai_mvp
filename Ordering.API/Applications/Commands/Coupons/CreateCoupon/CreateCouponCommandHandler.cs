using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.Domain.Aggregates.CouponAggregate;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Commands.Coupons.CreateCoupon;

public class CreateCouponCommandHandler(
    ILogger<CreateCouponCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCouponCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateCouponCommandHandler> _logger = logger; 
    public async Task<Result> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    { 
        try
        { 
            if (await _unitOfWork.Coupons.GetByCodeAsync(request.CouponCode) is not null)
            { 
                return Result.Failure("Coupon is already exists", ResponseStatus.Conflict);
            }

            var coupon = new Coupon(request.CouponCode, request.Parameter, request.Currency, request.Value, request.MinimumOrderValue);

            await _unitOfWork.Coupons.CreateAsync(coupon);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

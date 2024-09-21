using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Commands.Coupons.UpdateCouponById;

public class UpdateCouponByIdCommandHandler(
    ILogger<UpdateCouponByIdCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCouponByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UpdateCouponByIdCommandHandler> _logger = logger;
    public async Task<Result> Handle(UpdateCouponByIdCommand request, CancellationToken cancellationToken)
    { 
        try
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.CouponId);
            if (coupon is null)
            { 
                return Result.Failure("Coupon is not found", ResponseStatus.NotFound);
            }

            coupon.Update(request.Parameter, request.Currency, request.Value, request.MinimumOrderValue);

            _unitOfWork.Coupons.Update(coupon);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
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

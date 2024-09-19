using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR; 
using Ordering.Domain.SeedWork; 

namespace Ordering.API.Applications.Commands.Coupons.DeleteCouponById;

public class DeleteCouponByIdCommandHandler(
    ILogger<DeleteCouponByIdCommandHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCouponByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteCouponByIdCommandHandler> _logger = logger;
    public async Task<Result> Handle(DeleteCouponByIdCommand request, CancellationToken cancellationToken)
    { 
        try
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.CouponId);
            if (coupon is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Coupon is not found", ResponseStatus.NotFound);
            } 

            _unitOfWork.Coupons.Delete(coupon);

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

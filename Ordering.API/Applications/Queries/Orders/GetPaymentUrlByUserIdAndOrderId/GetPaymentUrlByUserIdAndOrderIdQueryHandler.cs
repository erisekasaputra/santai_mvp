using Core.Exceptions;
using Core.Messages;
using Core.Results;
using MediatR;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Queries.Orders.GetOrderByIdAndUserId;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetPaymentUrlByUserIdAndOrderId;

public class GetPaymentUrlByUserIdAndOrderIdQueryHandler(
    ILogger<GetOrderByIdAndUserIdQueryHandler> logger,
    IUnitOfWork unitOfWork,
    IPaymentService paymentService) : IRequestHandler<GetPaymentUrlByUserIdAndOrderIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetOrderByIdAndUserIdQueryHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;
    public async Task<Result> Handle(GetPaymentUrlByUserIdAndOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAndUserIdNoTrackingAsync(request.OrderId, request.BuyerId, cancellationToken);

            if (order is null)
            {
                return Result.Failure("Data not found", ResponseStatus.NotFound);
            }

            string paymentUrl = _paymentService.GeneratePaymentUrl(
                new SenangPayPaymentRequest(
                    order.Id,
                    order.GetDetail(),
                    order.Buyer.Name,
                    order.Buyer.Email,
                    order.Buyer.PhoneNumber,
                    order.GrandTotal.Amount));
              

            return Result.Success(new PaymentUrlResponseDto(paymentUrl), ResponseStatus.Ok);
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

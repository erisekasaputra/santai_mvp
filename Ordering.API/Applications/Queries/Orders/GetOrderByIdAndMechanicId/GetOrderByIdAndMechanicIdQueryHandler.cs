using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR; 
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions;
using Ordering.Domain.SeedWork;

namespace Ordering.API.Applications.Queries.Orders.GetOrderByIdAndMechanicId;

public class GetOrderByIdAndMechanicIdQueryHandler(
    ILogger<GetOrderByIdAndMechanicIdQueryHandler> logger,
    IUnitOfWork unitOfWork,
    IPaymentService paymentService) : IRequestHandler<GetOrderByIdAndMechanicIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<GetOrderByIdAndMechanicIdQueryHandler> _logger = logger;
    private readonly IPaymentService _paymentService = paymentService;
    public async Task<Result> Handle(GetOrderByIdAndMechanicIdQuery request, CancellationToken cancellationToken)
    { 
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null || order.Mechanic is null || order.Mechanic.MechanicId != request.UserId)
            {
                return Result.Failure("Order not found", ResponseStatus.NotFound);
            } 

            string paymentUrl = _paymentService.GeneratePaymentUrl(
                order.GetDetail(),
                order.GrandTotal.Amount,
                order.Id,
                order.Buyer.Name,
                order.Buyer.Email ?? string.Empty,
                order.Buyer.PhoneNumber ?? string.Empty);

            var orderResponse = order.ToOrderResponseDto();
            orderResponse.SetPaymentUrl(paymentUrl);
            return Result.Success(orderResponse, ResponseStatus.Ok);
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

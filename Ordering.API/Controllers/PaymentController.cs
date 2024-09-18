using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.CustomAttributes;
using Ordering.API.Extensions;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/v1/payment")]
public class PaymentController
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;
    public PaymentController(
        IUserInfoService userInfoService,
        ILogger<PaymentController> logger,
        IMediator mediator)
    {
        _userInfoService = userInfoService;
        _logger = logger; 
        _mediator = mediator;
    }


    [Authorize]
    [HttpPost]
    [Idempotency(nameof(CreatePayment))]
    public async Task<IResult> CreatePayment(
        [FromBody] PaymentRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();

            if (userClaim is null)
            {
                return TypedResults.Forbid();
            } 
             
            var command = new PayOrderPaymentByOrderIdCommand(
                request.OrderId,
                request.Amount,
                request.Currency,
                request.PaidAt,
                request.PaymentMethod,
                request.BankReference);

            var result = await _mediator.Send(command);

            return result.ToIResult();

        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}

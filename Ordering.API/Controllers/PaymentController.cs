using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Applications.Commands.Orders.PayOrderPaymentByOrderId;
using Ordering.API.Applications.Dtos.Requests; 
using Ordering.API.CustomAttributes; 

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/v1/payment")]
public class PaymentController
{ 
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator; 
    public PaymentController( 
        ILogger<PaymentController> logger,
        IMediator mediator)
    { 
        _logger = logger; 
        _mediator = mediator; 
    }


    [AllowAnonymous]
    [HttpPost("callback-senangpay")]
    public async Task<IResult> CreatePayment(
        [FromBody] SenangPayPaymentRequest request,
        [FromServices] IValidator<SenangPayPaymentRequest> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid) 
            {
                return TypedResults.BadRequest(validation.Errors);
            }

            var command = new PayOrderPaymentByOrderIdCommand(
                request.OrderId, 
                request.Amount,
                request.Method,
                request.TransactionId,
                request.Message,
                request.Hash,
                request.Status);

            var result = await _mediator.Send(command); 
            if (result.IsSuccess) 
            {
                return TypedResults.Ok("OK");
            }
            else
            {
                return TypedResults.InternalServerError("FAILED");
            } 
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}

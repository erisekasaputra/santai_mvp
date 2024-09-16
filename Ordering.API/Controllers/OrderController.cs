using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.CustomAttributes; 

namespace Ordering.API.Controllers;


[ApiController]
[Route("api/v1/orders")]

public class OrderController : ControllerBase
{
    private readonly IAccountServiceAPI _accountService;
    private readonly IMediator _mediator;
    private readonly IEncryptionService _kmsService;
    private readonly ILogger<OrderController> _logger;
    private readonly IUserInfoService _userInfoService;

    public OrderController(
        IAccountServiceAPI accountService,
        IMediator mediator,
        IEncryptionService kmsService,
        ILogger<OrderController> logger,
        IUserInfoService userInfoService)
    {
        _accountService = accountService;
        _mediator = mediator;
        _kmsService = kmsService;
        _logger = logger;
        _userInfoService = userInfoService; 
    } 


    [Authorize]
    [HttpPost]
    [Idempotency(nameof(CreateOrder))]
    public async Task<IResult> CreateOrder(
        [FromBody] OrderRequest request) 
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo(); 
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            } 

            for(int i = 0; i < 20; i++)
            {
                await _mediator.Send(
                new CreateOrderCommand(
                    userClaim.Sub,
                    userClaim.CurrentUserType,
                    request.AddressLine,
                    request.Latitude,
                    request.Longitude,
                    request.Currency,
                    request.IsScheduled,
                    request.ScheduledAt,
                    request.CouponCode,
                    request.LineItems,
                    request.Fleets)); 
            }

            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    } 
}

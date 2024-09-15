using Core.Enumerations;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
    public async Task<IResult> CreateOrder() 
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();

            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var lineItemId1 = Guid.Parse("15F26DC0-091C-4436-A99E-D11A49E7E67E");
            var fleetId1 = Guid.Parse("BFDC3596-3333-4C2C-A4E0-E5D50B506866"); 

            var lineItems = new List<LineItemRequest>
            {
                new (lineItemId1, 1)
            };

            var fleets = new List<FleetRequest>()
            {
                new(fleetId1)
            }; 

            var createResult = await _mediator.Send(
                new CreateOrderCommand(
                    userClaim.Sub, 
                    UserType.RegularUser,
                    "Jalan hayam wuruk",
                    -8.143145,
                    112.209617,
                    Currency.MYR,
                    false,
                    null,
                    null,
                    lineItems,
                    fleets));

            return TypedResults.Ok(new
            {
                CreateResult = createResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured, Error: {error}, Detail: {detail}", ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}

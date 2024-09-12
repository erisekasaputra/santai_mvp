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

    public OrderController(
        IAccountServiceAPI accountService,
        IMediator mediator,
        IEncryptionService kmsService,
        ILogger<OrderController> logger)
    {
        _accountService = accountService;
        _mediator = mediator;
        _kmsService = kmsService;
        _logger = logger;
    }

    [HttpPost]
    [Idempotency(nameof(CreateOrder))]
    public async Task<IResult> CreateOrder()
    {
        try
        {
            await Task.Delay(1);
            return TypedResults.Ok(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured, Error: {error}, Detail: {detail}", ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet]
    [OutputCache(Duration = 5)]
    public async Task<IResult> GetOrderById(Guid orderId)
    {
        try
        {
            await Task.Delay(1);
            return TypedResults.Ok(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured, Error: {error}, Detail: {detail}", ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet("test")]
    public async Task<IResult> Test()
    {
        return await Task.FromResult(TypedResults.Ok("Good"));
    }


    [Authorize]
    [HttpGet("test-create")]
    public async Task<IResult> TestCreate() 
    {
        try
        {
            var lineItemId1 = Guid.Parse("efebd86d-21e3-4f85-9449-b0aa95ac8ab3");
            var fleetId1 = Guid.NewGuid();
            var buyerId = Guid.NewGuid();

            var lineItems = new List<LineItemRequest>
            {
                new (lineItemId1, 10)
            };

            var fleets = new List<FleetRequest>()
            {
                new(fleetId1)
            }; 

            var createResult = await _mediator.Send(
                new CreateOrderCommand(
                    buyerId, 
                    UserType.RegularUser,
                    "Jalan hayam wuruk",
                    1.001,
                    1.1,
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

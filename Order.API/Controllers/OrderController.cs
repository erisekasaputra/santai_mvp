using Core.Enumerations;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Order.API.Applications.Commands.Orders.CreateOrder;
using Order.API.Applications.Dtos.Requests;
using Order.API.Applications.Services.Interfaces;
using Order.API.CustomAttributes;
using Order.API.Extensions; 

namespace Order.API.Controllers;


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
    public async Task<IResult> TestCreate()
    {
        try
        {
            var lineItems = new List<LineItemRequest>
            {
                new (Guid.NewGuid(), 10)
            };

            var fleets = new List<FleetRequest>()
            {
                new(Guid.NewGuid())
            };

            var result = await _mediator.Send(
                new CreateOrderCommand(
                    Guid.NewGuid(),
                    "Jalan hayam wuruk",
                    1.001,
                    1.1,
                    UserType.RegularUser,
                    false,
                    null,
                    null,
                    lineItems,
                    fleets));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured, Error: {error}, Detail: {detail}", ex.Message, ex.InnerException?.Message);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}

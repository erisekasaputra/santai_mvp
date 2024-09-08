using Core.Messages;
using Core.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching; 
using Order.API.Applications.Services.Interfaces;
using Order.API.CustomAttributes;
using Order.API.SeedWorks; 

namespace Order.API.Controllers;


[ApiController]
[Route("api/v1/orders")]

public class OrderController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMediator _mediator;
    private readonly IKeyManagementService _kmsService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IAccountService accountService,
        IMediator mediator, 
        IKeyManagementService kmsService,
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
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.API.Applications.Commands.Orders.CreateOrder;
using Order.Domain.Interfaces;

namespace Order.API.Controllers;


[ApiController]
[Route("api/v1/orders")]

public class OrderController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMediator _mediator;

    public OrderController(IAccountService accountService, IMediator mediator)
    {
        _accountService = accountService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IResult> GetMe()
    {
        //var timeZone = await _accountService.GetTimeZoneByUserIdAsync(Guid.NewGuid(), "RegularUser");
        //var result = await _mediator.Send(new CreateOrderCommand());
        return TypedResults.Ok("");
    }
}

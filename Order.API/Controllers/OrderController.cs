using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.API.Applications.Commands.Orders.CreateOrder;
using Order.Domain.Interfaces;
using Order.Infrastructure;

namespace Order.API.Controllers;


[ApiController]
[Route("api/v1/orders")]

public class OrderController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMediator _mediator;

    private readonly OrderDbContext _dbContext;

    public OrderController(IAccountService accountService, IMediator mediator, OrderDbContext dbContext)
    {
        _accountService = accountService;
        _mediator = mediator;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IResult> GetMe()
    {
        await Task.Delay(1);
        //var timeZone = await _accountService.GetTimeZoneByUserIdAsync(Guid.NewGuid(), "RegularUser");
        //var result = await _mediator.Send(new CreateOrderCommand());
        return TypedResults.Ok("");
    }
}

using Microsoft.AspNetCore.Mvc;
using Order.Domain.Interfaces;

namespace Order.API.Controllers;


[ApiController]
[Route("api/v1/orders")]

public class OrderController : ControllerBase
{
    private readonly IAccountService _accountService;

    public OrderController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IResult> GetMe()
    {
        var timeZone = await _accountService.GetTimeZoneByUserIdAsync(Guid.NewGuid(), "RegularUser");

        return await Task.FromResult(TypedResults.Ok(timeZone));
    }
}

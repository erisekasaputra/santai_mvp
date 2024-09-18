using Core.Dtos;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Applications.Commands.Orders.CancelOrderByBuyer;
using Ordering.API.Applications.Commands.Orders.CancelOrderByMechanic; 
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Commands.Orders.SetMechanicArriveByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetMechanicDispatchByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetOrderRatingByOrderIdAndUserId;
using Ordering.API.Applications.Commands.Orders.SetServiceFailedByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetServiceInProgressByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetServiceSuccessByOrderIdAndMechanicId;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Queries.Orders.GetOrderByIdAndUserId;
using Ordering.API.Applications.Queries.Orders.GetOrderSecretByOrderId;
using Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.CustomAttributes;
using Ordering.API.Extensions;

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
    [HttpGet("test")]
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


            List<Result> results = new List<Result>();
            for (int i = 1; i <= 10; i++)
            {
                results.Add(await _mediator.Send(
                    new CreateOrderCommand(
                        userClaim.Sub,
                        userClaim.CurrentUserType,
                        "Karangsono",
                        -8.143145,
                        112.2096,
                        Core.Enumerations.Currency.MYR,
                        false,
                        null,
                        "",
                        new List<LineItemRequest>() 
                        {
                            new LineItemRequest(Guid.Parse("6BCDFD23-836D-491F-A6D8-A08930923E93"), 1000)
                        },
                        new List<FleetRequest>() 
                        {
                            new FleetRequest(Guid.Parse("906B1E7D-0F67-4330-8357-18DEA8DED146"))
                        })));
            }

            return TypedResults.Ok(results);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
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

            var result = await _mediator.Send(
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

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet("{orderId}/secret")]
    public async Task<IResult> GetOrderSecret(
        Guid orderId)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetOrderSecretByOrderIdByUserIdQuery(orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet]
    public async Task<IResult> GetPaginatedOrders(
        [AsParameters] PaginatedRequestDto request)
    {

        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetPaginatedOrdersByUserIdQuery(userClaim.Sub, request.PageNumber, request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IResult> GetOrderById(
        Guid orderId)
    { 
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetOrderByIdAndUserIdQuery(orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/mechanic/cancel")]
    public async Task<IResult> CancelOrderByMechanic(
        Guid orderId)
    { 
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new CancelOrderByMechanicCommand(orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/buyer/cancel")]
    public async Task<IResult> CancelOrderByUser(
        Guid orderId)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new CancelOrderByBuyerCommand(
                    orderId,
                    userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        } 
    }

    [HttpPatch("{orderId}/service/fleet/{fleetId}/start")]
    public async Task<IResult> SetServiceInProgress(
        Guid orderId,
        Guid fleetId,
        [FromBody] OrderSecretRequest request)
    { 
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetServiceInProgressByOrderIdAndMechanicIdCommand(
                    orderId, userClaim.Sub, request.Secret, fleetId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/service/fleet/{fleetId}/success")]
    public async Task<IResult> SetServiceCompleted(
        Guid orderId,
        Guid fleetId,
        [FromBody] OrderSecretRequest request)
    {

        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetServiceSuccessByOrderIdAndMechanicIdCommand(orderId, userClaim.Sub, request.Secret, fleetId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpPatch("{orderId}/service/fleet/{fleetId}/failed")]
    public async Task<IResult> SetServiceIncompleted(
        Guid orderId,
        Guid fleetId,
        [FromBody] OrderSecretRequest request)
    {

        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetServiceFailedByOrderIdAndMechanicIdCommand(orderId, userClaim.Sub, fleetId, request.Secret));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPost("{orderId}/rating")]
    public async Task<IResult> SetOrderRating(
        Guid orderId,
        [FromBody] RatingRequestDto request)
    { 
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetOrderRatingByOrderIdAndUserIdCommand(
                    orderId, userClaim.Sub, request.Value, request.Comment, request.Images ?? []));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpPatch("{orderId}/mechanic/dispatch")]
    public async Task<IResult> SetDispatchMechanic(
        Guid orderId)
    {

        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetMechanicDispatchByOrderIdAndMechanicIdCommand(
                    orderId, userClaim.Sub));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/mechanic/arrive")]
    public async Task<IResult> SetArriveMechanic(
        Guid orderId)
    {

        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetMechanicArriveByOrderIdAndMechanicIdCommand(
                    orderId, userClaim.Sub));

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

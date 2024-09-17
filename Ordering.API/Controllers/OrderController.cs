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

    [HttpPatch("{orderId}/service/start")]
    public async Task<IResult> SetServiceInProgress(
        Guid orderId,
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
                    orderId, userClaim.Sub, request.Secret));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/service/success")]
    public async Task<IResult> SetServiceCompleted(
        Guid orderId,
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
                new SetServiceSuccessByOrderIdAndMechanicIdCommand(orderId, userClaim.Sub, request.Secret));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpPatch("{orderId}/service/failed")]
    public async Task<IResult> SetServiceIncompleted(
        Guid orderId,
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
                new SetServiceFailedByOrderIdAndMechanicIdCommand(orderId, userClaim.Sub, request.Secret));

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

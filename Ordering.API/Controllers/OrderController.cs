using Core.CustomAttributes;
using Core.CustomMessages;
using Core.Dtos;
using Core.Enumerations;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Applications.Commands.Orders.CalculateOrder;
using Ordering.API.Applications.Commands.Orders.CancelOrderByBuyer;
using Ordering.API.Applications.Commands.Orders.CancelOrderByMechanic;
using Ordering.API.Applications.Commands.Orders.CreateOrder;
using Ordering.API.Applications.Commands.Orders.PayCancellationRefundByOrderId;
using Ordering.API.Applications.Commands.Orders.SetMechanicArriveByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetMechanicDispatchByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetOrderFleetBasicInspection;
using Ordering.API.Applications.Commands.Orders.SetOrderFleetJobChecklist;
using Ordering.API.Applications.Commands.Orders.SetOrderFleetPreServiceInspection;
using Ordering.API.Applications.Commands.Orders.SetOrderRatingByOrderIdAndUserId;
using Ordering.API.Applications.Commands.Orders.SetServiceFailedByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetServiceInProgressByOrderIdAndMechanicId;
using Ordering.API.Applications.Commands.Orders.SetServiceSuccessByOrderIdAndMechanicId;
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Queries.Orders.GetOrderByIdAndMechanicId;
using Ordering.API.Applications.Queries.Orders.GetOrderByIdAndUserId;
using Ordering.API.Applications.Queries.Orders.GetOrderSecretByOrderId;
using Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByMechanicUserId;
using Ordering.API.Applications.Queries.Orders.GetPaginatedOrdersByUserId;
using Ordering.API.Applications.Queries.Orders.GetPaginatedOrderServiceActiveByUserId;
using Ordering.API.Applications.Queries.Orders.GetPaymentUrlByUserIdAndOrderId;
using Ordering.API.Applications.Services.Interfaces; 
using Ordering.API.Extensions;
using Ordering.Domain.Enumerations;

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
    private readonly IMasterDataServiceAPI _masterDataServiceAPI;

    public OrderController(
        IAccountServiceAPI accountService,
        IMediator mediator,
        IEncryptionService kmsService,
        ILogger<OrderController> logger,
        IUserInfoService userInfoService,
        IMasterDataServiceAPI masterDataServiceAPI)
    {
        _accountService = accountService;
        _mediator = mediator;
        _kmsService = kmsService;
        _logger = logger;
        _userInfoService = userInfoService;
        _masterDataServiceAPI = masterDataServiceAPI;
    }
     

    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
    [HttpPost]
    [Idempotency(nameof(CreateOrder))]
    public async Task<IResult> CreateOrder(
        [FromBody] OrderRequest request,
        [FromServices] IValidator<OrderRequest> requestValidation) 
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo(); 
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var validationResult = await requestValidation.ValidateAsync(request); 
            if (!validationResult.IsValid) 
            {
                return TypedResults.BadRequest(validationResult);
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
                    request.Fleets,
                    request.GrandTotal));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
    [HttpPost("calculate")] 
    public async Task<IResult> CalculateOrder(
        [FromBody] CalculateOrderRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            } 

            var result = await _mediator.Send(
                new CalculateOrderCommand(
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
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
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
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
    public async Task<IResult> GetPaginatedOrders( 
        [FromQuery] PaginatedRequestDto request, [FromQuery] OrderStatus? orderStatus = null)
    { 
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetPaginatedOrdersByUserIdQuery((userClaim.CurrentUserType == UserType.Administrator ? null : userClaim.Sub), request.PageNumber, request.PageSize, orderStatus));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpGet("mechanic")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
    public async Task<IResult> GetPaginatedOrdersByMechanic(
        [FromQuery] PaginatedRequestDto request, [FromQuery] OrderStatus? orderStatus = null)
    {
        try
        {
            Console.WriteLine(request.PageNumber);
            Console.WriteLine(request.PageSize);

            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetPaginatedOrdersByMechanicUserIdQuery(userClaim.Sub, request.PageNumber, request.PageSize, orderStatus));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [HttpGet("active")]
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
    public async Task<IResult> GetPaginatedOrderServiceActiveByUserId()
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new GetPaginatedOrderServiceActiveByUserIdQuery(userClaim.Sub));

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
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
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

    [HttpGet("{orderId}/mechanic")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
    public async Task<IResult> GetOrderByIdAndMechanicId(
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
                new GetOrderByIdAndMechanicIdQuery(orderId, userClaim.Sub));

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
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
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

    [HttpGet("{orderId}/payment-url")]
    [Authorize]
    public async Task<IResult> GetPaymentUrl(
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
                new GetPaymentUrlByUserIdAndOrderIdQuery(
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

    [HttpPatch("{orderId}/service/fleet/{fleetId}/start")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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



    [HttpPatch("{orderId}/service/fleet/{fleetId}/basic-inspection")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
    public async Task<IResult> UpdateBasicInspection(
       Guid orderId,
       Guid fleetId,
       [FromBody] BasicInspectionsRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetOrderFleetBasicInspectionCommand(
                    orderId, fleetId, request.BasicInspections));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }




    [HttpPatch("{orderId}/service/fleet/{fleetId}/pre-service-inspection")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
    public async Task<IResult> UpdatePreServiceInspection(
       Guid orderId,
       Guid fleetId,
       [FromBody] PreServiceInspectionsRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(
                new SetOrderFleetPreServiceInspectionCommand(
                    orderId, fleetId, request.PreServiceInspections));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/service/fleet/{fleetId}/job-checklist")]
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
    public async Task<IResult> UpdateJobChecklist(
       Guid orderId,
       Guid fleetId,
       [FromBody] JobChecklistsRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            if (string.IsNullOrEmpty(request.Comment) || !request.JobChecklists.Any())
            {
                return TypedResults.BadRequest(
                    Result.Failure("Please fill out all the blanks", ResponseStatus.BadRequest));
            }

            var result = await _mediator.Send(
                new SetOrderFleetJobChecklistCommand(
                    orderId, fleetId, request.JobChecklists, request.Comment));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPatch("{orderId}/cancellation/refund/pay")]
    [Authorize(Policy = "AdministratorUserOnlyPolicy")]
    public async Task<IResult> PayCancellationRefund(
       Guid orderId, 
       [FromBody] PayCancellationRefundRequest request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(new PayCancellationRefundByOrderIdCommand(orderId, request.Amount, request.Currency));

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
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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


    [HttpPut("{orderId}/rating")]
    [Authorize(Policy = "BusinessStaffRegularUserPolicy")]
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
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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
    [Authorize(Policy = "MechanicUserOnlyPolicy")]
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

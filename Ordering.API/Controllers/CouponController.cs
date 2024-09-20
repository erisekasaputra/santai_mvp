using Core.Dtos;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MassTransit.Internals.GraphValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Applications.Commands.Coupons.CreateCoupon;
using Ordering.API.Applications.Commands.Coupons.DeleteCouponById;
using Ordering.API.Applications.Commands.Coupons.UpdateCouponById;
using Ordering.API.Applications.Queries.Coupons.GetCouponByCode;
using Ordering.API.Applications.Queries.Coupons.GetCouponById;
using Ordering.API.Applications.Queries.Coupons.GetPaginatedCoupons;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions; 

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/v1/coupons")]
public class CouponController : ControllerBase
{
    private readonly IAccountServiceAPI _accountService;
    private readonly IMediator _mediator;
    private readonly IEncryptionService _kmsService;
    private readonly ILogger<OrderController> _logger;
    private readonly IUserInfoService _userInfoService; 
    public CouponController(
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
    [HttpGet]
    public async Task<IResult> GetPaginatedCoupons(
        [FromBody] PaginatedRequestDto request)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(new GetPaginatedCouponsQuery(request.PageNumber, request.PageSize));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [Authorize]
    [HttpGet("{couponId}")]
    public async Task<IResult> GetCouponById(
        Guid couponId)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(new GetCouponByIdQuery(couponId));

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [Authorize]
    [HttpGet("code/{code}")]
    public async Task<IResult> GetCouponByCode(
       string code)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(new GetCouponByCodeQuery(code));

            return result.ToIResult();
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
    public async Task<IResult> CreateCoupon(
        [FromBody] CreateCouponCommand command)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(command);

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [Authorize()]
    [HttpPut]
    public async Task<IResult> UpdateCoupon(
        [FromBody] UpdateCouponByIdCommand command)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(command);

            return result.ToIResult();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }

    [Authorize()]
    [HttpDelete] 
    public async Task<IResult> DeleteCoupon(
        [FromBody] DeleteCouponByIdCommand command)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var result = await _mediator.Send(command);

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

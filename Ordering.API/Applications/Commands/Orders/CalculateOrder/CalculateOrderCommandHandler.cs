using Core.CustomMessages;
using Core.Exceptions;
using Core.Extensions;
using Core.Results; 
using Core.Utilities; 
using MediatR; 
using Ordering.API.Applications.Dtos.Responses;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions;
using Ordering.Domain.Aggregates.CouponAggregate;
using Ordering.Domain.Aggregates.FleetAggregate;
using Ordering.Domain.Aggregates.OrderAggregate; 
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Commands.Orders.CalculateOrder;

public class CalculateOrderCommandHandler( 
    ILogger<CalculateOrderCommandHandler> logger,
    IUnitOfWork unitOfWork, 
    IMasterDataServiceAPI masterDataServiceAPI) : IRequestHandler<CalculateOrderCommand, Result>
{ 
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CalculateOrderCommandHandler> _logger = logger; 
    private readonly IMasterDataServiceAPI _masterDataServiceAPI = masterDataServiceAPI;

    public async Task<Result> Handle(CalculateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                coupon = await _unitOfWork.Coupons.GetByCodeAsync(request.CouponCode); 
            }

            var initialMaster = await _masterDataServiceAPI.GetMasterDataInitializationMasterResponseDto();
            if (initialMaster is null)
            {
                _logger.LogError("Initial master create order is null, master service did not retrive correct data");
                return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            return CreateOrderByRelatedUserType(
                request,
                initialMaster,
                coupon);
        } 
        catch (Exception ex)
        {
            if (ex is ArgumentNullException or DomainException or InvalidDateOperationException or InvalidOperationException or PriceChangesException)
            {
                return Result.Failure(ex.Message, ResponseStatus.BadRequest);
            }

            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static Result CreateOrderByRelatedUserType(
        CalculateOrderCommand command, 
        MasterDataInitializationMasterResponseDto master,
        Coupon? coupon)
    {  
        var order = new Order(
            command.Currency,
            command.Address,
            command.Latitude,
            command.Longitude,
            command.BuyerId,
            "Calculation", 
            "Calculation",
            "Calculation",
            command.BuyerType,
            command.IsOrderScheduled,
            command.ScheduledOn); 

        foreach (var lineItem in command.LineItems ?? [])
        { 
            order.AddOrderItem(
                new(lineItem.Id,
                    order.Id,
                    "Calculation",
                    "Calculation",
                    lineItem.Price,
                    lineItem.Currency,
                    lineItem.Quantity));
        }
          
        var masterBasicInspection = master.BasicInspections.Select(
            x => (x.Description, x.Parameter.CleanAndLowering(), x.Value));

        var masterPreServiceInspection = master.PreServiceInspections.Select(
            x => (x.Description, x.Parameter.CleanAndLowering(), x.Rating,
            x.PreServiceInspectionResults.Select(b => (b.Description, b.Parameter.CleanAndLowering(), b.IsWorking))));
        
        var masterJobChecklist = master.BasicInspections.Select(
         x => (x.Description, x.Parameter.CleanAndLowering(), false));

        foreach (var fleet in command.Fleets ?? [])
        {
            order.AddFleet(
                new Fleet(
                    order.Id,
                    fleet.Id,
                    "Calculation",
                    "Calculation",
                    "Calculation",
                    "Calculation"), 
                masterBasicInspection, 
                masterPreServiceInspection,
                masterJobChecklist);
        } 

        if (!string.IsNullOrWhiteSpace(command.CouponCode) && coupon is not null)
        {
            if (coupon.CouponValueType == PercentageOrValueType.Percentage)
            {
                order.ApplyDiscount(
                    Discount.CreatePercentageDiscount(
                        order.Id,
                        command.CouponCode,
                        coupon.ValuePercentage,
                        coupon.MinimumOrderValue,
                        coupon.Currency));
            }
            else if (coupon.CouponValueType == PercentageOrValueType.Value)
            {
                order.ApplyDiscount(
                    Discount.CreateValueDiscount(
                        order.Id,
                        command.CouponCode,
                        coupon.ValueAmount,
                        coupon.Currency,
                        coupon.MinimumOrderValue));
            }
        }  
            
        foreach (var fee in master.Fees)
        {
            if (fee.Parameter == PercentageOrValueType.Percentage)
            {
                order.ApplyFee(Fee.CreateByPercentage(order.Id, fee.FeeDescription, fee.ValuePercentage, fee.Currency));
            }
            else if (fee.Parameter == PercentageOrValueType.Value)
            {
                order.ApplyFee(Fee.CreateByValue(order.Id, fee.FeeDescription, fee.ValueAmount, fee.Currency));
            }
        }

        order.SetSecretKey("Calculation");

        order.CalculateGrandTotal(); 

        var orderDto = order.ToOrderResponseDto(); 
        return Result.Success(orderDto, ResponseStatus.Ok);
    } 
}

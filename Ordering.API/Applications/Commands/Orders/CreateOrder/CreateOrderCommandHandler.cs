using Core.Enumerations;
using Core.Events;
using Core.Events.Catalog;
using Core.Events.Ordering;
using Core.Exceptions;
using Core.Extensions;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using Core.Utilities;
using MassTransit;
using MediatR; 
using Ordering.API.Applications.Dtos.Requests;
using Ordering.API.Applications.Dtos.Responses; 
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Extensions;
using Ordering.Domain.Aggregates.CouponAggregate;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;
using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork; 
using System.Data; 
namespace Ordering.API.Applications.Commands.Orders.CreateOrder;
public class CreateOrderCommandHandler(
    IPaymentService paymentService,
    ILogger<CreateOrderCommandHandler> logger, 
    IUnitOfWork unitOfWork,
    IAccountServiceAPI accountService,
    ICatalogServiceAPI catalogService,
    IPublishEndpoint publishEndpoint,
    IEncryptionService encryptionService,
    IMasterDataServiceAPI masterDataServiceAPI) : IRequestHandler<CreateOrderCommand, Result>
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger; 
    private readonly IAccountServiceAPI _accountService = accountService; 
    private readonly ICatalogServiceAPI _catalogService = catalogService;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IMasterDataServiceAPI _masterDataServiceAPI = masterDataServiceAPI;


    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var requestItems = command.LineItems;
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 
        try
        {
            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(command.CouponCode))
            { 
                coupon = await _unitOfWork.Coupons.GetByCodeAsync(command.CouponCode);
                if (coupon is null)
                {
                    await RollbackAsync(cancellationToken);
                    return Result.Failure("Coupon code does not exist", ResponseStatus.NotFound)
                        .WithError(new("Coupon.Code", "Coupon not found"));
                }
            }


            (var buyer, var isAccountResponseSuccess) = await _accountService.GetUserDetail<AccountIdentityResponseDto>(command.BuyerId, command.Fleets.Select(x => x.Id));

            if (!isAccountResponseSuccess)
            {
                if (buyer is null || buyer.Data is null)
                {
                    await RollbackAsync(cancellationToken);
                    return  Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
                }

                await RollbackAsync(cancellationToken);
                return Result.Failure(buyer.Message ?? Messages.UnknownError, ResponseStatus.BadRequest);
            }

            if (buyer is null || buyer.Data is null)
            {
                await RollbackAsync(cancellationToken);
                return Result.Failure("User data not found", ResponseStatus.NotFound);
            }


            if (buyer.Data.UnknownFleets.Any())
            {
                await RollbackAsync(cancellationToken);
                return Result.Failure("There are severals fleet that does not exist", 
                    ResponseStatus.UnprocessableEntity)
                    .WithError(new ErrorDetail("Fleets", "There are severals fleets not found"))
                    .WithData(new 
                    {
                        Ids = buyer.Data.UnknownFleets
                    });
            }

            var initialMaster = await _masterDataServiceAPI.GetMasterDataInitializationMasterResponseDto();  
            if (initialMaster is null)
            {
                _logger.LogError("Initial master create order is null, Master data did not retrive correct data");
                await RollbackAsync(cancellationToken);
                return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            return await CreateOrderByRelatedUserType(
                command,
                buyer?.Data?.Fullname,
                buyer?.Data?.Email,
                buyer?.Data?.PhoneNumber,
                buyer?.Data?.Fleets,
                buyer?.Data?.TimeZoneId,
                initialMaster,
                coupon,
                cancellationToken); 
        }
        catch (InvalidDateOperationException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (AccountServiceHttpRequestException ex)
        { 
            LoggerHelper.LogError(_logger, ex);
            await RollbackAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (CatalogServiceHttpRequestException ex)
        {  
            LoggerHelper.LogError(_logger, ex);
            await RollbackAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (PaymentServiceHttpRequestException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (ArgumentNullException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (DomainException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (NotImplementedException ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken); 
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            await RollbackAndRecoveryStockAsync(requestItems, cancellationToken); 
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    } 


    private async Task<Result> CreateOrderByRelatedUserType(
        CreateOrderCommand command,  
        string? buyerName,
        string? buyerEmail,
        string? buyerPhoneNumber,
        IEnumerable<AccountIdentityFleetResponseDto>? fleets,
        string? timeZoneId,
        MasterDataInitializationMasterResponseDto master,
        Coupon? coupon,
        CancellationToken cancellationToken)
    {
        var lineItems = command.LineItems.Select(x => (x.Id, x.Quantity)); 
         
        if (fleets is null || !fleets.Any() || string.IsNullOrWhiteSpace(timeZoneId)) 
        { 
            await RollbackAsync(cancellationToken);
            return Result.Failure("User data not valid", ResponseStatus.NotFound);
        } 

        (var items, var isCatalogResponseSuccess) = await _catalogService.SubstractStockAndGetDetailItems(lineItems);

        if (!isCatalogResponseSuccess)
        {
            if (items is null)
            {
                await RollbackAsync(cancellationToken);
                return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            await RollbackAsync(cancellationToken);
            return Result.Failure(items.Message ?? Messages.UnknownError, ResponseStatus.UnprocessableEntity)
                .WithError(new ErrorDetail("LineItems", "There are severals line items could not be processed"))
                .WithData(items.Data);
        }

        var order = new Order(  
            command.Currency,
            command.Address,
            command.Latitude,
            command.Longitude,
            command.BuyerId,
            buyerName ?? string.Empty,
            buyerEmail,
            buyerPhoneNumber,
            command.BuyerType,
            command.IsOrderScheduled,
            command.ScheduledOn.FromLocalToUtc(timeZoneId)); 

        order.SetSecretKey(
            await _encryptionService.EncryptAsync($"{order.Id}{order.Buyer.BuyerId}{order.GrandTotal.Amount}"));

        foreach (var lineItem in items?.Data ?? [])
        {
            if (IsNullCatalogItems(lineItem.Name, lineItem.Sku, lineItem.Price, lineItem.Currency))
            {
                await RollbackAndRecoveryStockAsync(command.LineItems, cancellationToken);
                return Result.Failure(
                    Messages.InternalServerError, ResponseStatus.InternalServerError);
            }

            order.AddOrderItem(
                new(lineItem.Id,
                    order.Id,
                    lineItem.Name!,
                    lineItem.Sku!,
                    lineItem.Price!.Value,
                    lineItem.Currency!.Value,
                    command.LineItems.First(x => x.Id == lineItem.Id).Quantity));
        }

        var masterBasicInspection = master.BasicInspections.Select(
            x => (x.Description, x.Parameter.CleanAndLowering(), x.Value));

        var masterPreServiceInspection = master.PreServiceInspections.Select(
            x => (x.Description, x.Parameter.CleanAndLowering(), x.Rating,
            x.PreServiceInspectionResults.Select(b => (b.Description, b.Parameter.CleanAndLowering(), b.IsWorking))));

        foreach (var fleet in fleets)
        {
            order.AddFleet(new(
                order.Id,
                fleet.Id,
                fleet.Brand,
                fleet.Model,
                fleet.RegistrationNumber,
                fleet.ImageUrl),
                masterBasicInspection,
                masterPreServiceInspection);
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
      
        //order.ApplyTax(new Tax(10, command.Currency));

        foreach(var fee in master.Fees) 
        {
            if (fee.Parameter == PercentageOrValueType.Percentage)
            { 
                order.ApplyFee(Fee.CreateByPercentage(order.Id, fee.FeeDescription, fee.ValuePercentage, fee.Currency)); 
            }
            else if(fee.Parameter == PercentageOrValueType.Value)
            {
                order.ApplyFee(Fee.CreateByValue(order.Id, fee.FeeDescription, fee.ValueAmount, fee.Currency)); 
            } 
        }

        order.CalculateGrandTotal(); 
        await _unitOfWork.Orders.CreateAsync(order, cancellationToken);

        if (order.IsShouldRequestPayment)
        {
            var requestPayment = new SenangPayPaymentRequest(
                order.Id,
                order.GetDetail(),
                order.Buyer.Name,
                order.Buyer.Email,
                order.Buyer.PhoneNumber,
                order.GrandTotal.Amount);

            string paymentUrl = _paymentService.GeneratePaymentUrl(requestPayment); 
            order.SetPaymentUrl(paymentUrl);
        }

        if (order.IsScheduled)
        {
            var scheduledOrderWorker = new ScheduledOrder(
                order.Id, 
                order.ScheduledOnUtc!.Value, 
                order.IsShouldRequestPayment);

            await _unitOfWork.ScheduledOrders.CraeteAsync(scheduledOrderWorker);
        }


        var orderDto = order.ToOrderResponseDto(); 
        await _unitOfWork.CommitTransactionAsync(cancellationToken); 
        return Result.Success(orderDto, ResponseStatus.Created);
    }

    private static bool IsNullCatalogItems(string? name, string? sku, decimal? price, Currency? currency)
    {
        if (string.IsNullOrWhiteSpace(name)) return true;
        if (string.IsNullOrWhiteSpace(sku)) return true;
        if (price is null || !price.HasValue) return true;
        if (currency is null || !currency.HasValue) return true;  
        return false;
    }

    private async Task RollbackAndRecoveryStockAsync(IEnumerable<LineItemRequest> items, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken); 

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken); 

        await _publishEndpoint.Publish(
            new OrderFailedRecoveryStockIntegrationEvent(
                items.Select(x => new CatalogItemStockIntegrationEvent(x.Id, x.Quantity))),
            cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);
    }

    private async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    }
}

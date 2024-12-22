using Ordering.API.Applications.Dtos.Responses;
using Ordering.Domain.Aggregates.FleetAggregate;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.Enumerations;
using Ordering.Domain.ValueObjects;

namespace Ordering.API.Extensions;

public static class Mapper
{
    public static OrderResponseDto ToOrderResponseDto(this Order order)
    {
        return new OrderResponseDto(
                order.Id,
                order.Secret,
                order.Address.AddressLine,
                order.Address.Latitude,
                order.Address.Longitude,
                order.Buyer.ToBuyerDto(),
                order.Mechanic?.ToMechanicDto(),
                order.LineItems.ToLineItemsDto(),
                order.Fleets.ToFleetsDto(),
                order.CreatedAtUtc,
                order.IsScheduled,
                order.ScheduledOnUtc,
                order.Payment?.ToPaymentDto(),
                order.PaymentUrl,
                order.PaymentExpiration,
                order.Currency,
                order.OrderAmount,
                order.Discount?.ToDiscountDto(),
                order.GrandTotal.Amount,
                order.Rating?.ToRatingDto(),
                order.RatingImages,
                order.Fees.ToFeesDto(),
                order.Cancellation?.ToCancellationDto(),
                order.Status,
                order.IsPaid,
                order.IsRated,
                order.IsPaymentExpire 
            ); 
    }
    public static IEnumerable<OrdersActiveResponseDto> ToOrderActivesDto(this IEnumerable<Order> orders)
    {
        foreach (var order in orders)
        {
            yield return order.ToOrderActiveDto();
        }
    }
    public static OrdersActiveResponseDto ToOrderActiveDto(this Order order)
    {
        List<string> statuses = ["Order Received", "Technician Discovered", "Servicing", "Completed"];
        string status;
        int step = 0;
        switch (order.Status)
        {
            case OrderStatus.PaymentPaid:
                statuses[0] = "Order Received";
                status = "Order Received";
                step = 0;
                break;
            case OrderStatus.FindingMechanic:
                statuses[0] = "Discovering Technician";
                status = "Discovering Technician";
                step = 0;
                break;
            case OrderStatus.MechanicAssigned:
                statuses[1] = "Technician Discovered";
                status = "Technician Discovered";
                step = 1;
                break;
            case OrderStatus.MechanicArrived:
                statuses[1] = "Technician Arrived";
                status = "Technician Arrived";
                step = 1;
                break; 
            case OrderStatus.MechanicDispatched:
                statuses[1] = "On The Way";
                status = "On The Way";
                step = 1;
                break; 
            case OrderStatus.ServiceInProgress:
                statuses[2] = "Servicing";
                status = "Servicing";
                step = 2;
                break; 
            case OrderStatus.ServiceIncompleted or OrderStatus.ServiceCompleted:
                statuses[3] = "Completed";
                status = "Completed";
                step = 3;
                break;
            default: 
                status = string.Empty; 
                break;
        }

        return new OrdersActiveResponseDto(order.Id, order.Secret, status, step, statuses);
    }
    public static BuyerResponseDto ToBuyerDto(this Buyer buyer)
    {
        return new BuyerResponseDto(
            buyer.BuyerId,
            buyer.Name,
            buyer.ImageUrl,
            buyer.Email,
            buyer.PhoneNumber);
    }

    public static MechanicResponseDto ToMechanicDto(this Mechanic mechanic)
    {
        return new MechanicResponseDto(
            mechanic.MechanicId,
            mechanic.Name,
            mechanic.ImageUrl,
            mechanic.Rating?.ToRatingDto(),
            mechanic.Performance,
            mechanic.IsRated);
    }

    public static IEnumerable<LineItemResponseDto> ToLineItemsDto(this ICollection<LineItem> lineItems)
    {
        foreach (var lineItem in lineItems)
        {
            yield return lineItem.ToLineItemDto();
        }
    }
    public static LineItemResponseDto ToLineItemDto(this LineItem lineItem)
    {
        return new LineItemResponseDto(
            lineItem.LineItemId,
            lineItem.Name,
            lineItem.Sku,
            lineItem.UnitPrice,
            lineItem.Quantity,
            lineItem.SubTotal.Currency,
            lineItem.Tax?.Rate,
            lineItem.Tax?.TaxAmount.Amount,
            lineItem.SubTotal.Amount);
    }

    public static IEnumerable<FleetResponseDto> ToFleetsDto(this ICollection<Fleet> fleets)
    {
        foreach (var fleet in fleets) 
        {
            yield return fleet.ToFleetDto();
        }
    }

    public static FleetResponseDto ToFleetDto(this Fleet fleet)
    {
        return new FleetResponseDto(
            fleet.FleetId,
            fleet.Brand,
            fleet.Model,
            fleet.RegistrationNumber,
            fleet.ImageUrl,
            fleet.BasicInspections.Select(x => new BasicInspectionResponseDto(x.Description, x.Parameter, x.Value)),
            fleet.PreServiceInspections.Select(x => new PreServiceInspectionResponseDto(x.Description, x.Parameter, x.Rating, x.PreServiceInspectionResults.Select(b => new PreServiceInspectionResultResponseDto(b.Description, b.Parameter, b.IsWorking)))),
            fleet.JobChecklists.Select(x => new JobChecklistResponseDto(x.Description, x.Parameter, x.Value)),
            fleet.Comment);
    }


    public static PaymentResponseDto ToPaymentDto(this Payment payment)
    {
        return new PaymentResponseDto();
    }
    public static DiscountResponseDto ToDiscountDto(this Discount coupon)
    {
        return new DiscountResponseDto(
            coupon.Id,
            coupon.CouponCode,
            coupon.CouponValueType,
            coupon.Currency,
            coupon.ValuePercentage,
            coupon.ValueAmount,
            coupon.MinimumOrderValue,
            coupon.DiscountAmount.Amount);
    }
    public static RatingResponseDto ToRatingDto(this Rating rating)
    {
        return new RatingResponseDto(rating.Value, rating.Comment);
    }

    public static IEnumerable<FeeResponseDto> ToFeesDto(this ICollection<Fee> fees)
    {
        foreach (var fee in fees) 
        {
            yield return fee.ToFeeDto();
        }
    }

    public static FeeResponseDto ToFeeDto(this Fee fee)
    {
        return new FeeResponseDto(
            fee.PercentageOrValueType,
            fee.FeeDescription,
            fee.Currency,
            fee.ValuePercentage,
            fee.ValueAmount,
            fee.FeeAmount.Amount);
    }


    public static CancellationResponseDto ToCancellationDto(this Cancellation cancellation)
    {
        return new CancellationResponseDto(
            cancellation.CancellationCharges.ToCancellationChargesDto(),
            cancellation.CancellationRefund?.Amount,
            cancellation.CancellationRefund?.Currency);
    } 

    public static IEnumerable<FeeResponseDto> ToCancellationChargesDto(this ICollection<CancellationFee> fees)
    {
        foreach (var fee in fees) 
        {
            yield return fee.ToCancellationChargeDto();
        }
    }

    public static FeeResponseDto ToCancellationChargeDto(this CancellationFee fee)
    { 
        return new FeeResponseDto(
            fee.PercentageOrValueType,
            fee.FeeDescription,
            fee.Currency,
            fee.ValuePercentage,
            fee.ValueAmount,
            fee.FeeAmount.Amount);
    }

}

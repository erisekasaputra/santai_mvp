using Core.Enumerations;
using Core.Exceptions;
using Core.ValueObjects;
using Order.Domain.Enumerations;  
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class LineItem : Entity
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public decimal UnitPrice { get; private set; }      
    public int Quantity { get; private set; } 
    public Tax? Tax { get; private set; } 
    public Guid OrderingId { get; private set; } 
    public Money SubTotal { get; private set; }
    public LineItem()
    {
        Name = string.Empty;
        Sku = string.Empty; 
        SubTotal = null!;
    }
    public LineItem(
        Guid id,
        Guid orderId,
        string name,
        string sku,
        decimal price,
        Currency currency,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU cannot be null or empty.");

        if (price <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Id = id;
        OrderingId = orderId;
        Name = name;
        Sku = sku;
        UnitPrice = price; 
        Quantity = quantity; 
        SubTotal = new Money(0, currency);
    }

    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
        { 
            throw new DomainException("Cannot add zero or negative quantity.");
        }

        Quantity += quantity;
    }

    public void ReduceQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Cannot reduce by zero or negative quantity.");

        if (Quantity < quantity)
            throw new DomainException("Quantity to reduce exceeds current quantity.");

        Quantity -= quantity;
    }

    public void ApplyDiscount(Coupon coupon)
    {
        if (coupon is null) 
            throw new DomainException("Coupon cannot be null."); 
        
        if (coupon.CouponValueType == PercentageOrValueType.Value)
        {
            if (coupon.ValueAmount <= 0)
            { 
                throw new DomainException($"Coupon value can not be empty.");
            }

            if (coupon.Currency != SubTotal.Currency)
            {
                throw new DomainException($"Coupon currency ({coupon.Currency}) does not match line item currency ({SubTotal.Currency}).");
            }
        } 
    }

    public void ApplyTax(Tax tax)
    {
        if (tax is null) 
            throw new DomainException("Tax cannot be null."); 

        if (tax.TaxAmount.Currency != SubTotal.Currency)
        {
            throw new DomainException("Tax currency should be the same as line item currency");
        }

        Tax = tax;
    }

    public Money CalculateTotalPrice()
    {
        var subtotal = new Money(UnitPrice * Quantity, SubTotal.Currency);  

        if (Tax is not null)
        {
            var taxAmount = Tax.Apply(subtotal);
            subtotal += taxAmount;
        }

        SubTotal = subtotal;

        return subtotal;
    } 
}
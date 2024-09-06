using Order.Domain.Enumerations;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class LineItem : Entity
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public Money UnitPrice { get; private set; }  
    public Money BaseUnitPrice { get; private init; }
    public int Quantity { get; private set; }
    public Coupon? Coupon { get; private set; }
    public Tax? Tax { get; private set; } 

    public LineItem(
        Guid id,
        string name,
        string sku,
        Money price,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU cannot be null or empty.");

        if (price.Amount <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Id = id;
        Name = name;
        Sku = sku;
        UnitPrice = price;
        BaseUnitPrice = price;
        Quantity = quantity;  
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
            if (coupon.Value is null)
            { 
                throw new DomainException($"Coupon value can not be empty.");
            }

            if (coupon.Value.Currency != BaseUnitPrice.Currency)
            {
                throw new DomainException($"Coupon currency ({coupon.Value?.Currency}) does not match line item currency ({BaseUnitPrice.Currency}).");
            }
        }

        Coupon = coupon;
    }

    public void ApplyTax(Tax tax)
    {
        if (tax is null) 
            throw new DomainException("Tax cannot be null."); 

        Tax = tax;
    }

    public Money CalculateTotalPrice()
    {
        var subtotal = new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);

        if (Coupon is not null)
        {
            var discount = Coupon.Apply(subtotal);
            subtotal -= discount;
        }

        if (Tax is not null)
        {
            var taxAmount = Tax.Apply(subtotal);
            subtotal += taxAmount;
        }

        return subtotal;
    } 
}
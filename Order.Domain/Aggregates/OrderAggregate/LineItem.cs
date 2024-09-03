using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.OrderAggregate;

public class LineItem : Entity
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Discount? Discount { get; private set; }
    public Tax? Tax { get; private set; }
    public decimal TotalPrice => CalculateTotalPrice();

    public LineItem(
        Guid id,
        string name,
        string sku,
        decimal unitPrice,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU cannot be null or empty.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Id = id;
        Name = name;
        Sku = sku;
        UnitPrice = unitPrice;
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

    public void ApplyDiscount(Discount discount)
    {
        if (discount is null) 
            throw new DomainException("Discount cannot be null."); 

        Discount = discount;
    }

    public void ApplyTax(Tax tax)
    {
        if (tax is null) 
            throw new DomainException("Tax cannot be null."); 

        Tax = tax;
    }

    private decimal CalculateTotalPrice()
    {
        var discountAmount = Discount is null ? 0 : Discount.Apply(UnitPrice * Quantity);
        var subtotal = (UnitPrice * Quantity) - discountAmount;

        if (subtotal < 1)
        {
            throw new DomainException("Subtotal can not less than or equal with zero");
        }

        var taxAmount = Tax is null ? 0 : Tax.Apply(subtotal);
        return subtotal + taxAmount;
    }
}
namespace Core.Exceptions;

public class PriceChangesException : Exception
{
    public decimal PriceShouldBe { get; set; } 
    public PriceChangesException(string message, decimal priceShouldBe) : base(message)
    {
        PriceShouldBe = priceShouldBe;
    }
}

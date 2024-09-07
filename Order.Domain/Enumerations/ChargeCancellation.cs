namespace Order.Domain.Enumerations;

public class ChargeCancellation
{
    public static readonly List<FeeDescription> Charges = [FeeDescription.ServiceFee, FeeDescription.MechanicFee];
}

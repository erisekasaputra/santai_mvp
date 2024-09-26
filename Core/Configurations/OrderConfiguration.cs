namespace Core.Configurations;

public class OrderConfiguration
{
    public const string SectionName = "Order";
    public int OrderMechanicConfirmTimeToAcceptInSeconds { get; set; } = 0;
    public int PenaltyInMinutes { get; set; } = 0;
}

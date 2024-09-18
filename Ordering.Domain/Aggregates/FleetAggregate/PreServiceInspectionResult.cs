using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class PreServiceInspectionResult : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid PreServiceInspectionId { get; set; }
    public string Description { get; set; }
    public string Parameter { get; set; }
    public bool IsWorking { get; set; }

    public PreServiceInspectionResult()
    {
        Description = string.Empty;
        Parameter = string.Empty;
    }

    public PreServiceInspectionResult(
        Guid orderId,
        Guid fleetId,
        Guid preServiceInspectionId,
        string description,
        string parameter,
        bool isWorking)
    {
        OrderId = orderId;
        FleetId = fleetId;
        PreServiceInspectionId = preServiceInspectionId;
        Description = description;
        Parameter = parameter;
        IsWorking = isWorking;
    }

    public void Update(bool isWorking)
    {
        IsWorking = isWorking;
    }
}

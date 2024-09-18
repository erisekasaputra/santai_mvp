using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class PreServiceInspectionResult : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid PreServiceInspectionId { get; set; }
    public string Parameter { get; set; }
    public bool IsWorking { get; set; }
    public PreServiceInspectionResult(
        Guid orderId,
        Guid fleetId,
        Guid preServiceInspectionId,
        string parameter,
        bool isWorking)
    {
        OrderId = orderId;
        FleetId = fleetId;
        PreServiceInspectionId = preServiceInspectionId;
        Parameter = parameter;
        IsWorking = isWorking;
    }

    public void Update(bool isWorking)
    {
        IsWorking = isWorking;
    }
}

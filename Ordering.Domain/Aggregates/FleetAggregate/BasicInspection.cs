using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class BasicInspection : Entity
{
    public Guid OrderId { get; private init; }
    public Guid FleetId { get; private init; }
    public Guid FleetAggregateId { get; private init; }
    public string Description { get; private init; }
    public string Parameter { get; private init; }
    public int Value { get; private set; }

    public BasicInspection()
    {
        Description = string.Empty;
        Parameter = string.Empty;
    }

    public BasicInspection(
        Guid orderId,   
        Guid fleetId,
        Guid fleetAggregateId,
        string description,
        string parameter, 
        int value)
    {
        Description = description;
        OrderId = orderId;
        FleetId = fleetId;
        FleetAggregateId = fleetAggregateId;
        Parameter = parameter;
        Value = value;
    }

    public void Update(int value)
    {
        Value = value;
    }
}

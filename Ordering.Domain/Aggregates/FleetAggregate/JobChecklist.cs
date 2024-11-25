
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class JobChecklist : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid FleetAggregateId { get; set; }
    public string Description { get; set; }
    public string Parameter { get; set; }
    public bool Value { get; set; }

    public JobChecklist()
    {
        Description = string.Empty;
        Parameter = string.Empty;
    }

    public JobChecklist(
        Guid orderId,
        Guid fleetId,
        Guid fleetAggregateId,
        string description,
        string parameter,
        bool value)
    {
        OrderId = orderId;
        FleetId = fleetId;
        FleetAggregateId = fleetAggregateId;
        Description = description;
        Parameter = parameter;
        Value = value;  
    } 

    public void UpdateValue(bool value)
    {
        Value = value;
    }
}

using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class PreServiceInspection : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid FleetIdAggregate { get; set; }
    public string Parameter { get; set; } 
    public int Rating { get; set; }
    public ICollection<PreServiceInspectionResult>  PreServiceInspectionResults { get; set; }

    public PreServiceInspection(
        Guid orderId,
        Guid fleetId,
        Guid fleetIdAggregate,
        string parameter,
        int rating)
    {
        OrderId = orderId;
        FleetId = fleetId;
        FleetIdAggregate = fleetIdAggregate;
        Parameter = parameter;
        Rating = rating;
        PreServiceInspectionResults = [];
    }

    public void AddPreServiceInspectionResult(string parameter, bool isWorking)
    {
        PreServiceInspectionResults ??= [];

        if (PreServiceInspectionResults.Any(x => x.Parameter == parameter))
        {
            return;
        }

        PreServiceInspectionResults.Add(new PreServiceInspectionResult(OrderId, FleetId, Id, parameter, isWorking));
    }

    public void UpdateRating(int rating)
    {
        Rating = rating;
    }
}

using Microsoft.Identity.Client;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class PreServiceInspection : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid FleetAggregateId { get; set; }
    public string Description { get; set; }
    public string Parameter { get; set; } 
    public int Rating { get; set; }
    public ICollection<PreServiceInspectionResult>  PreServiceInspectionResults { get; set; }

    public PreServiceInspection()
    {
        Description = string.Empty;
        Parameter = string.Empty;
        PreServiceInspectionResults = [];
    }
    public PreServiceInspection(
        Guid orderId,
        Guid fleetId,
        Guid fleetAggregateId,
        string description,
        string parameter,
        int rating,
        IEnumerable<(string description, string parameter, bool isWorking)> inspectionResults)
    {
        OrderId = orderId;
        FleetId = fleetId;
        FleetAggregateId = fleetAggregateId;
        Description = description;
        Parameter = parameter;
        Rating = rating;
        PreServiceInspectionResults = [];

        foreach (var inspectionResult in inspectionResults)
        {
            if (PreServiceInspectionResults.Any(x => x.Parameter == inspectionResult.parameter))
            {
                continue;
            }

            PreServiceInspectionResults.Add(new(
                orderId,
                fleetId,
                Id,
                inspectionResult.description,
                inspectionResult.parameter,
                inspectionResult.isWorking));
        }
    }

    public bool PutPreServiceInspectionResult(string parameter, bool isWorking)
    {
        PreServiceInspectionResults ??= [];

        var preServiceInspectionResult = PreServiceInspectionResults.FirstOrDefault(x => x.Parameter == parameter); 
        if (preServiceInspectionResult is null)
        {
            return false;
        }

        preServiceInspectionResult.Update(isWorking);
        return true;
    }

    public void UpdateRating(int rating)
    {
        Rating = rating;
    }
}

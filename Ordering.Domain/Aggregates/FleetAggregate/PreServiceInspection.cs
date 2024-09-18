using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class PreServiceInspection : Entity
{
    public Guid OrderId { get; set; }
    public Guid FleetId { get; set; }
    public Guid FleetIdAggregate { get; set; }
    public string Description { get; set; }
    public string Parameter { get; set; } 
    public int Rating { get; set; }
    public ICollection<PreServiceInspectionResult>  PreServiceInspectionResults { get; set; }

    public PreServiceInspection(
        Guid orderId,
        Guid fleetId,
        Guid fleetIdAggregate,
        string description,
        string parameter,
        int rating,
        IEnumerable<(string description, string parameter, bool isWorking)> inspectionResults)
    {
        OrderId = orderId;
        FleetId = fleetId;
        FleetIdAggregate = fleetIdAggregate;
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

    public void PutPreServiceInspectionResult(string parameter, bool isWorking)
    {
        PreServiceInspectionResults ??= [];

        var preServiceInspectionResult = PreServiceInspectionResults.FirstOrDefault(x => x.Parameter == parameter); 
        if (preServiceInspectionResult is null)
        {
            return;
        }

        preServiceInspectionResult.Update(isWorking);
    }

    public void UpdateRating(int rating)
    {
        Rating = rating;
    }
}

using Ordering.Domain.Enumerations;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.FleetAggregate;

public class Fleet : Entity
{
    public Guid FleetId { get; set; }
    public Guid OrderId { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public string? ImageUrl { get; private set; } 
    public InspectionStatus InspectionStatus { get; private set; }
    public ICollection<BasicInspection> BasicInspections { get; private set; }
    public ICollection<PreServiceInspection> PreServiceInspections { get; private set; }
    public ICollection<JobChecklist> JobChecklists { get; private set; }
    public string Comment { get; private set; }
    public Fleet()
    {
        Brand = string.Empty;
        Model = string.Empty;
        RegistrationNumber = string.Empty;
        ImageUrl = string.Empty;
        InspectionStatus = InspectionStatus.BasicInspection;
        BasicInspections = [];
        PreServiceInspections = [];
        JobChecklists = [];
        Comment = string.Empty;
    }

    public Fleet(
        Guid orderingId,
        Guid fleetId,
        string brand,
        string model,
        string registrationNumber,
        string? imageUrl)
    {
        OrderId = orderingId;
        FleetId = fleetId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
        InspectionStatus = InspectionStatus.BasicInspection;
        BasicInspections = [];
        PreServiceInspections = [];
        JobChecklists = [];
        Comment = string.Empty;
    }
    public void UpdateComment(string comment)
    {
        Comment = comment;
    }

    public void AddBasicInspectionDefault(
        string description,
        string parameter,
        int value)
    {
        BasicInspections ??= [];
        var basicInspection = BasicInspections.FirstOrDefault(x => x.Parameter == parameter);
        if (basicInspection is not null)
        {
            return;
        }

        BasicInspections.Add(new BasicInspection(OrderId, FleetId, Id, description, parameter, value));
    }

    public bool PutBasicInspection(string parameter, int value)
    {
        BasicInspections ??= [];
        var basicInspection = BasicInspections.FirstOrDefault(x => x.Parameter == parameter); 
        if (basicInspection is null) 
        {
            return false;
        }

        basicInspection.Update(value);
        return true;
    }

    public void AddPreServiceInspectionDefault(
        string description,
        string parameter, 
        int rating,
        IEnumerable<(string description, string parameter, bool isWorking)> preInspectionResults)
    {
        PreServiceInspections ??= [];

        var check = PreServiceInspections.FirstOrDefault(x => x.Parameter == parameter);
        if (check is not null)
        {
            return;
        }

        var preServiceInspection = new PreServiceInspection(
            OrderId,
            FleetId,
            Id,
            description,
            parameter,
            rating,
            preInspectionResults);

        PreServiceInspections.Add(preServiceInspection); 
    }

    public bool PutPreServiceInspection(
        string parameter, 
        int rating, 
        IEnumerable<(string parameter, bool isWorking)> preInspectionResults)
    {
        PreServiceInspections ??= []; 

        var preServiceInspection = PreServiceInspections.FirstOrDefault(x => x.Parameter == parameter); 
        if (preServiceInspection is null) 
        {
            return false;
        }

        preServiceInspection.UpdateRating(rating); 
        foreach (var item in preInspectionResults)
        {
            if (!preServiceInspection.PutPreServiceInspectionResult(item.parameter, item.isWorking))
            {
                return false;
            }
        }

        return true;
    }

    public bool PutJobChecklist(string parameter, bool value)
    {
        JobChecklists ??= [];
        var jobChecklist = JobChecklists.FirstOrDefault(x => x.Parameter == parameter);
        if (jobChecklist is null)
        {
            return false;
        }

        jobChecklist.UpdateValue(value);
        return true;
    }

    public void AddJobChecklist(
       string description,
       string parameter,
       bool value)
    {
        JobChecklists ??= [];

        var check = JobChecklists.FirstOrDefault(x => x.Parameter == parameter);
        if (check is not null)
        {
            return;
        }

        var preServiceInspection = new JobChecklist(
            OrderId,
            FleetId,
            Id,
            description,
            parameter,
            value);

        JobChecklists.Add(preServiceInspection);
    }

}

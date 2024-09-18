using Core.Exceptions;
using Microsoft.EntityFrameworkCore;
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
    public ICollection<BasicInspection> BasicInspections { get; private set; } = [];
    public ICollection<PreServiceInspection> PreServiceInspections { get; private set; } = [];

    public Fleet()
    {
        Brand = string.Empty;
        Model = string.Empty;
        RegistrationNumber = string.Empty;
        ImageUrl = string.Empty;
        InspectionStatus = InspectionStatus.BasicInspection;
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
    }

    public void PutBasicInspection(string parameter, int value)
    {
        BasicInspections ??= []; 
        if (BasicInspections.Any(x => x.Parameter == parameter)) 
        {
            return;
        }

        var basicInspection = new BasicInspection(OrderId, FleetId, Id, parameter, value);
        basicInspection.SetEntityState(EntityState.Added);
        BasicInspections.Add(basicInspection);
    }

    public void PutPreServiceInspection(
        string parameter, 
        int rating, 
        ICollection<(string parameter, bool isWorking)> preInspectionResults)
    {
        PreServiceInspections ??= []; 

        var preServiceInspection = PreServiceInspections.FirstOrDefault(x => x.Parameter == parameter); 
        if (preServiceInspection is not null) 
        { 
            preServiceInspection.UpdateRating(rating);

            foreach (var item in preInspectionResults)
            {
                preServiceInspection.AddPreServiceInspectionResult(item.parameter, item.isWorking);
            } 

            return; 
        }




        var preInspection = new PreServiceInspection(OrderId, FleetId, Id, parameter, rating); 
        if (preInspectionResults.Count == 0)
        {
            throw new DomainException("Pre-Inspection result can not be empty");
        }

        foreach (var item in preInspectionResults)
        {
            preInspection.AddPreServiceInspectionResult(item.parameter, item.isWorking);
        }

        PreServiceInspections.Add(preInspection);
    }


    public void UpdatePreServiceInspection()
    {

    }
}

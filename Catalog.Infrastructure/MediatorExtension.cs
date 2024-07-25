using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Infrastructure;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, CatalogDbContext dbContext)
    { 
        var domainEntities = dbContext.ChangeTracker
            .Entries()
            .Where(e => e.Entity is Entity<object> entity &&
                        entity.DomainEvents is not null &&
                        entity.DomainEvents.Count > 0)
            .ToList();
         
        var domainEvents = domainEntities
            .SelectMany(e => ((Entity<object>)e.Entity).DomainEvents)
            .ToList();
         
        domainEntities.ForEach(e => ((Entity<object>)e.Entity).ClearDomainEvents());
         
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}

using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.Events;
using Catalog.Domain.Exceptions;
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.BrandAggregate;

public class Brand : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public ICollection<Item> Items { get; set; } = []; 

    public bool IsDeleted { get; private set; }
    
    public Brand(string name, string imageUrl)
    {
        Name = name;
        ImageUrl = imageUrl; 
        IsDeleted = false;
    }
    public void Update(string name, string imageUrl)
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not update data once the data is deleted");
        }

        Name = name;
        ImageUrl = imageUrl;

        RaiseBrandUpdatedDomainEvent(this);
    }
    
    public void Delete()
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not delete data once the data is already deleted");
        }

        IsDeleted = true;

        RaiseBrandDeletedDomainEvent(Id);
    }  

    private void RaiseBrandUpdatedDomainEvent(Brand brand)
    {
        AddDomainEvent(new BrandUpdatedDomainEvent(brand));
    }

    private void RaiseBrandDeletedDomainEvent(string id)
    {
        AddDomainEvent(new BrandDeletedDomainEvent(id));
    }
}

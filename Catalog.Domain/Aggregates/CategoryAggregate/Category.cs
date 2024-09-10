using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.Events; 
using Catalog.Domain.SeedWork;
using Core.Exceptions;

namespace Catalog.Domain.Aggregates.CategoryAggregate;

public class Category(string name, string imageUrl) : Entity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string ImageUrl { get; private set; } = imageUrl;

    
    public ICollection<Item> Items = []; 

    public bool IsDeleted { get; private set; }

    public void Update(string name, string imageUrl)
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not update data once the data is deleted");
        }

        Name = name;
        ImageUrl = imageUrl;

        RaiseCategoryUpdatedDomainEvent(this);
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new DomainException("Can not delete data once the data is already deleted");
        }

        IsDeleted = true;

        RaiseCategoryDeletedDomainEvent(Id);
    }

    private void RaiseCategoryUpdatedDomainEvent(Category category)
    {
        AddDomainEvent(new CategoryUpdatedDomainEvent(category));
    }
    private void RaiseCategoryDeletedDomainEvent(Guid id)
    {
        AddDomainEvent(new CategoryDeletedDomainEvent(id));
    }
}

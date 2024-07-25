using Catalog.Domain.Aggregates.ItemAggregate; 
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.CategoryAggregate;

public class Category(string name, string imageUrl) : Entity, IAggregateRoot
{
    public string Name { get; set; } = name;
    public string ImageUrl { get; set; } = imageUrl;

    public ICollection<Item> Items = [];
}

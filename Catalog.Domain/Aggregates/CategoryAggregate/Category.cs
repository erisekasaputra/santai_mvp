using Catalog.Domain.Aggregates.ItemAggregate; 
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.CategoryAggregate;

public class Category : Entity<string>, IAggregateRoot
{ 
    public string Name { get; set; } 
    public string ImageUrl { get; set; }  
    public ICollection<Item> Items = [];
    public Category(string id, string name, string imageUrl)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
    }
}

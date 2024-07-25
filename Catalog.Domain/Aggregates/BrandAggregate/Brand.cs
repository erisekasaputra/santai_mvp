using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.SeedWork;

namespace Catalog.Domain.Aggregates.BrandAggregate;

public class Brand(string name, string imageUrl) : Entity, IAggregateRoot
{
    public string Name { get; set; } = name;
    public string ImageUrl { get; set; } = imageUrl;

    public ICollection<Item> Items { get; set; } = [];
}

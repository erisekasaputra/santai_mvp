namespace Catalog.API.Commands.Brands.CreateBrand;
public class CreateBrandResponse(
    string id,
    string name,
    string imageUrl)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ImageUrl { get; set; } = imageUrl;
    public static CreateBrandResponse Create(
        string id,
        string name,
        string imageUrl)
    {
        return new CreateBrandResponse(
            id,
            name,
            imageUrl);
    }
}

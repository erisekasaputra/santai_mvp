namespace Catalog.API.Commands.Categories.CreateCategory;
public class CreateCategoryResponse(
    string id,
    string name,
    string imageUrl)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ImageUrl { get; set; } = imageUrl;
    public static CreateCategoryResponse Create(
        string id,
        string name,
        string imageUrl)
    {
        return new CreateCategoryResponse(
            id,
            name,
            imageUrl);
    }
}

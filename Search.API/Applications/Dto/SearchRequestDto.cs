namespace Search.API.Applications.Dto;

public class SearchRequestDto
{
    public required string CategoryId { get; set; } 
    public required string? BrandId { get; set; }
    public int PageNumber { get; private set; }

    public SearchRequestDto(string categoryId, string? brandId, int pageNumber)
    {
        CategoryId = categoryId.Trim();
        BrandId = brandId?.Trim();
        PageNumber = pageNumber < 1 ? 1 : pageNumber; 
    }
}

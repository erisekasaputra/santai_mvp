namespace Catalog.API.DTOs.PaginatedResponseDto;

public class PaginatedResponseDto<T>(int pageNumber, int pageSize, int pageCount, int totalPages, List<T> items)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int PageCount { get; set; } = pageCount;
    public int TotalPages { get; set; } = totalPages;
    public List<T> Items { get; set; } = items;
}

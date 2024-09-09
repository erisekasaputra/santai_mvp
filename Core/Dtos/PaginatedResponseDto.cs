namespace Core.Dtos;

public class PaginatedResponseDto<T>(int pageNumber, int pageSize, int pageCount, int totalPages, IEnumerable<T> items)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int PageCount { get; set; } = pageCount;
    public int TotalPages { get; set; } = totalPages;
    public IEnumerable<T> Items { get; set; } = items; 
}

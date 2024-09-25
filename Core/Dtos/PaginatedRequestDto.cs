namespace Core.Dtos;

public class PaginatedRequestDto
{
    public const string PageNumberName = nameof(PageNumber);
    public const string PageSizeName = nameof(PageSize);

    public required int PageNumber { get; set; } = 1;
    public required int PageSize { get; set; } = 10;

    public PaginatedRequestDto()
    {
        
    }

    public PaginatedRequestDto(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

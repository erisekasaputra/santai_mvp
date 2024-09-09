namespace Core.Dtos;

public class PaginatedRequestDto
{
    public const string PageNumberName = nameof(PageNumber);
    public const string PageSizeName = nameof(PageSize);

    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 10; 

    public PaginatedRequestDto(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

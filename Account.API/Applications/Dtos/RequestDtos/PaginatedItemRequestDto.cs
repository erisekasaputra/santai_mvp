namespace Account.API.Applications.Dtos.RequestDtos;

public class PaginatedItemRequestDto
{
    private const int MaxPageSize = 100;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value > 0 && value <= int.MaxValue / 10) ? value : 1;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > 0 && value <= MaxPageSize) ? value : MaxPageSize;
    }

    public PaginatedItemRequestDto(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
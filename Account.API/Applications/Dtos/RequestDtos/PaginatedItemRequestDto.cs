namespace Account.API.Applications.Dtos.RequestDtos;

public record PaginatedItemRequestDto(int PageNumber = 1, int PageSize = 10);

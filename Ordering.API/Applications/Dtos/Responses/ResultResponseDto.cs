namespace Ordering.API.Applications.Dtos.Responses;

public class ResultResponseDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

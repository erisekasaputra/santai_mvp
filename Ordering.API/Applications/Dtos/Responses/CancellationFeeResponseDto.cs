namespace Ordering.API.Applications.Dtos.Responses;

public class CancellationFeeResponseDto
{
    public List<string> CancellationParameters { get; private set; }
    public CancellationFeeResponseDto(
        List<string> cancellationParameters)
    {
        CancellationParameters = cancellationParameters;
    }
}

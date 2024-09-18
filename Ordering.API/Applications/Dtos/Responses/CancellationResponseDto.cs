using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class CancellationResponseDto
{ 
    public IEnumerable<FeeResponseDto>? CancellationCharges { get; set; }
    public decimal? CancellationRefundAmount { get; set; }
    public Currency? Currency { get; set; }

    public CancellationResponseDto(
        IEnumerable<FeeResponseDto>? cancellationCharges,
        decimal? cancellationRefundAmount,
        Currency? currency)
    {
        CancellationCharges = cancellationCharges;
        CancellationRefundAmount = cancellationRefundAmount;
        Currency = currency;
    }
}

using Core.Enumerations;

namespace Ordering.API.Applications.Dtos.Responses;

public class CancellationResponseDto
{ 
    public IEnumerable<FeeResponseDto>? CancellationCharges { get; private set; }
    public decimal? CancellationRefundAmount { get; private set; }
    public Currency? Currency { get; private set; }

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

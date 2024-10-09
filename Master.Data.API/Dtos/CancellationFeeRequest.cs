using Master.Data.API.Models;

namespace Master.Data.API.Dtos;

public class CancellationFeeRequest
{
    public IEnumerable<Fee> CancellationFees { get; set; }

    public CancellationFeeRequest(
        IEnumerable<Fee> cancellationFees)
    {
        CancellationFees = cancellationFees;
    }
}

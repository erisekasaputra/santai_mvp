using Master.Data.API.Models;

namespace Master.Data.API.Dtos;

public class ServiceFeeRequest
{
    public IEnumerable<Fee> ServiceFees { get; set; }
    public ServiceFeeRequest(
        IEnumerable<Fee> serviceFees)
    {
        ServiceFees = serviceFees;
    }
}

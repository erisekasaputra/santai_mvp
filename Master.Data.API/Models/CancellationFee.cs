namespace Master.Data.API.Models;

public class CancellationFee
{
    public List<string> CancellationParameters { get; set; } = [];

    public CancellationFee(
        List<string> cancellationParameters)
    {
        CancellationParameters = cancellationParameters;
    }
}

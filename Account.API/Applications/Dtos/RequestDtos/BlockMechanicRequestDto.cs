namespace Account.API.Applications.Dtos.RequestDtos;

public class BlockMechanicRequestDto
{
    public string Reason { get; set; }
    public BlockMechanicRequestDto(string reason)
    {
        Reason = reason;
    }
}

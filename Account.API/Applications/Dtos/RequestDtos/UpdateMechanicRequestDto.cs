using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;
  
public class UpdateMechanicRequestDto(AddressRequestDto address, string timeZoneId)
{ 
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}


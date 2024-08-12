using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateStaffRequestDto(string name, AddressRequestDto address, string timeZoneId)
{
    public string Name { get; } = name.Clean();
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}

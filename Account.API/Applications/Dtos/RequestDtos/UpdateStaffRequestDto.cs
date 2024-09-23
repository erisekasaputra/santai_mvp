using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateStaffRequestDto(string name, AddressRequestDto address, string timeZoneId)
{
    public required string Name { get; set; } = name.Clean();
    public required AddressRequestDto Address { get; set; } = address;
    public required string TimeZoneId { get; set; } = timeZoneId.Clean();
}
